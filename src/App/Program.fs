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
            routeCi "/status" >=> text "ok"
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
            >=> mustBeLoggedIn
            >=> Controller.upsertPage
        ]
        POST
        >=> mustBeLoggedIn
        >=> choose [
            routeCi "/upsert" >=> Controller.upsert
        ]
        DELETE
        >=> mustBeLoggedIn
        >=> choose [
            routeCif "/article/%i" Controller.deleteArticle
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
            | None -> return Results.Redirect($"/articles/upsert", false)
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
    Func<HttpRequest, IRepository, _>(fun (request: HttpRequest) (repository: IRepository) ->
        task {
            let (tagExists, tag) = request.Query.TryGetValue "tag"

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

funGroup.MapGet("/about", Func<_>(fun _ -> task { return About.view () }))
|> ignore

let feedResult (feedFn: string -> Model.ArticleStub list -> XDocument) (ctx: HttpContext) repository =
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

        ctx.Response.ContentType <- "application/atom+xml"

        return Results.Text(xml, "application/rss+xml", Encoding.UTF8)
    }

app.MapGet("/atom", Func<HttpContext, IRepository, _>(feedResult Syndication.syndicationFeed))
|> ignore

app.MapGet("/rss", Func<HttpContext, IRepository, _>(feedResult Syndication.channelFeed))
|> ignore

app.Run("http://0.0.0.0:5000")
