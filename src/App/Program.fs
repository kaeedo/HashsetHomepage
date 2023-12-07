open System
open System.IO
open System.Threading.Tasks
open DataAccess
open Hashset
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Giraffe
open App.Views.Pages
open Markdig
open Markdown.ColorCode
open System.Text
open System.Xml.Linq
open Model
#if !DEBUG
open Microsoft.Extensions.FileProviders

#endif

let builder =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")
    WebApplication.CreateBuilder(WebApplicationOptions(ContentRootPath = contentRoot, WebRootPath = webRoot))

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables()
|> ignore

let services = builder.Services

let conf =
    services
        .BuildServiceProvider()
        .GetService<IConfiguration>()

let repository = Repository(conf.["ConnectionString"]) :> IRepository
repository.Migrate()

services.AddTransient<IRepository>(fun _ -> repository)
|> ignore

services.AddTransient<IFileStorage, FileStorage>()
|> ignore

services.Configure<ForwardedHeadersOptions>(fun (options: ForwardedHeadersOptions) ->
    options.ForwardedHeaders <-
        ForwardedHeaders.XForwardedFor
        ||| ForwardedHeaders.XForwardedProto

    options.KnownNetworks.Clear()
    options.KnownProxies.Clear())
|> ignore

services
    .AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null)
|> ignore

services.AddTransient<IUserService, UserService>()
|> ignore

#if !DEBUG
services.AddWebOptimizer() |> ignore
#endif
services.AddControllersWithViews() |> ignore
services.AddHttpContextAccessor() |> ignore
services.AddGiraffe() |> ignore


let mustBeLoggedIn: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        requiresAuthentication (Giraffe.Auth.challenge "BasicAuthentication") next ctx

let version =
    Reflection.Assembly
        .GetEntryAssembly()
        .GetName()
        .Version

let webApp =
    choose [
        GET
        >=> choose [
            routeCi "/version" >=> text (version.ToString())
            routeCi "/old/status" >=> text "ok" // done
            routeCi "/old" >=> Controller.homepage // done
            routeCi "/old/articles" >=> Controller.articles // done
            routeCif "/old/article/%i_%s" (fun (id, _) -> Controller.article id) // done
            routeCif "/old/article/%i" Controller.articleRedirect // done
            routeCi "/old/about" >=> Controller.about // done
            routeCi "/old/rss" // done
            >=> setHttpHeader "Content-Type" "application/rss+xml"
            >=> Controller.rss
            routeCi "/old/atom" // done
            >=> setHttpHeader "Content-Type" "application/atom+xml"
            >=> Controller.atom

            routeCi "/old/articles/upsert"
            //>=> mustBeLoggedIn
            >=> Controller.upsertPage
        ]
        POST
        >=> choose [
            routeCi "/old/upsert" >=> Controller.upsert
        ]
        DELETE
        //>=> mustBeLoggedIn
        >=> choose [
            routeCif "/old/article/%i" Controller.deleteArticle
        ]
    ]

let app = builder.Build()

app
#if !DEBUG
    .UseWebOptimizer()
#endif
    .UseStaticFiles()
    .UseAuthentication()
    .UseHttpsRedirection()
    .Use(
        Func<HttpContext, RequestDelegate, _>(fun ctx next ->
            (task {
                ctx.Response.Headers.Add("X-Clacks-Overhead", "GNU Terry Pratchett")
                return! next.Invoke ctx
            }
            :> Task))
    )
    .UseGiraffe(webApp)

// https://github.com/albertwoo/FunBlazorSSRDemo
let funGroup = app.MapGroup("").AddFunBlazor()

funGroup.MapGet(
    "",
    Func<IRepository, _>(fun (repository: IRepository) ->
        task {
            // TODO: replace getLatestArticle with only get latest slug
            let! latestArticle = Articles.getLatestArticle repository

            match latestArticle with
            | None -> return Results.Redirect("/articles/upsert", false)
            | Some la -> return Results.Redirect($"/article/{App.Utils.getUrl la.Id la.Title}", false)
        })
)
|> ignore

funGroup.MapGet(
    "/article/{slug:regex(^(\d+)_.*$)}",
    Func<IRepository, string, _>(fun (repository: IRepository) (slug: string) ->
        task {
            let articleId = slug.Substring(0, slug.IndexOf('_')) |> int
            let! article = Articles.getArticle repository articleId

            let pipeline =
                MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseColorCode()
                    .Build()

            let html = Markdown.ToHtml(article.Source, pipeline)

            let article = { article with Document = html }

            return Article.view article
        })
)
|> ignore

funGroup.MapGet(
    "/article/{articleId:int}",
    Func<IRepository, int, _>(fun (repository: IRepository) (articleId: int) ->
        task {
            let! article = Articles.getArticle repository articleId

            return Results.Redirect($"/article/{App.Utils.getUrl article.Id article.Title}", true, true)
        })
)
|> ignore

funGroup.MapGet(
    "/articles",
    Func<HttpContext, IRepository, _>(fun (ctx: HttpContext) (repository: IRepository) ->
        task {
            let (tagExists, tag) = ctx.Request.Query.TryGetValue "tag"

            let! articles =
                if tagExists then
                    Articles.getArticlesByTag repository (tag.ToString())
                else
                    Articles.getArticles repository

            let articles =
                articles
                |> Seq.toList
                |> List.map Articles.getArticleStub

            return (ArticleList.view articles)
        })
)
|> ignore

funGroup.MapGet(
    "/articles/upsert",
    Func<HttpContext, IRepository, IFileStorage, _>(fun ctx repository fileStore ->
        task {
            let! articles = repository.GetAllArticles()

            let articleIds =
                articles
                |> Seq.map (fun (pd: ParsedDocument) ->
                    let id = pd.Id
                    let title = pd.Title
                    id, title)

            let! upsertDocument =
                let (idExists, id) = ctx.Request.Query.TryGetValue "id"

                if idExists then
                    task {
                        let! article = repository.GetArticleById <| int (id.ToString())

                        return {
                            UpsertDocument.ExistingIds = articleIds
                            Title = article.Title
                            Source = article.Source
                            Description = article.Description
                            ArticleDate = article.ArticleDate
                            Tags = article.Tags |> List.map (fun t -> t.Name)
                        }
                    }
                else
                    task {
                        return {
                            UpsertDocument.ExistingIds = articleIds
                            Title = String.Empty
                            Source = String.Empty
                            Description = String.Empty
                            ArticleDate = DateTime.UtcNow.Date
                            Tags = []
                        }
                    }

            return Upsert.view upsertDocument (fileStore.GetImages() |> Seq.toList)
        })
)
|> ignore

app.MapPost(
    "/upsert",
    Func<HttpContext, IRepository, IFileStorage, _>
        (fun (ctx: HttpContext) (repository: IRepository) (fileStorage: IFileStorage) ->
            task {
                let document = {
                    UpsertDocument.Title = ctx.Request.Form["Title"].ToString()
                    Description = ctx.Request.Form["Description"].ToString()
                    ArticleDate = DateTime.Parse(ctx.Request.Form["ArticleDate"].ToString())
                    Source = ctx.Request.Form["Source"].ToString()
                    Tags = ctx.Request.Form["Tags"].ToArray() |> Array.toList
                    ExistingIds = []
                }

                let! parsedDocument = Articles.parse document

                ctx.Request.Form.Files
                |> Seq.filter (fun f -> f.Length > 0L)
                |> Seq.iter (fun f -> fileStorage.SaveFile f.FileName f.CopyToAsync)

                let id = int <| ctx.Request.Form.["Id"].Item 0

                if id = 0 then
                    do! Articles.addArticle repository parsedDocument document.Tags
                    ctx.Response.Headers.Add("HX-Location", "/")
                else
                    do! Articles.updateArticle repository id parsedDocument document.Tags
                    ctx.Response.Headers.Add("HX-Location", $"/article/%s{App.Utils.getUrl id parsedDocument.Title}")

                return Results.Created()
            })
)
|> ignore

app.MapDelete(
    "/article/{articleId:int}",
    Func<HttpContext, IRepository, int, _>(fun (ctx: HttpContext) (repository: IRepository) (articleId: int) ->
        task {
            do! Articles.deleteArticleById repository articleId

            ctx.Response.Headers.Add("HX-Location", "/articles/upsert")

            return Results.Accepted()
        })
)
|> ignore

funGroup.MapGet("/about", Func<_>(fun _ -> task { return About.view () }))
|> ignore

let feedResult
    (feedFn: string -> Model.ArticleStub list -> XDocument)
    (feedType: string)
    (ctx: HttpContext)
    repository
    =
    task {
        let (tagExists, tag) = ctx.Request.Query.TryGetValue "tag"

        let! articles =
            if tagExists then
                Articles.getArticlesByTag repository (tag.ToString())
            else
                Articles.getArticles repository

        let articles =
            articles
            |> Seq.map Articles.getArticleStub
            |> List.ofSeq

        let host = ctx.Request.Host.Value

        let xml = (feedFn host articles).ToString()

        return Results.Text(xml, $"application/{feedType}+xml", Encoding.UTF8)
    }

app.MapGet("/atom", Func<HttpContext, IRepository, _>(feedResult Syndication.syndicationFeed "atom"))
|> ignore

app.MapGet("/rss", Func<HttpContext, IRepository, _>(feedResult Syndication.channelFeed "rss"))
|> ignore

app.Map("/status", Func<_>(fun _ -> Results.Text("ok")))
|> ignore

app.Run("http://0.0.0.0:5000")
