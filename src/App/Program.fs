open System
open System.IO
open System.Threading.Tasks
open App
open App.Views.Partials
open DataAccess
open Hashset
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Components.Authorization
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.HttpOverrides
open App.Views.Pages
open System.Text
open System.Xml.Linq
open Model
open Npgsql
open Supabase.Gotrue
open Supabase.Gotrue.Interfaces

let builder =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")
    WebApplication.CreateBuilder(WebApplicationOptions(ContentRootPath = contentRoot, WebRootPath = webRoot))

let services = builder.Services

services.AddNpgsqlDataSource(builder.Configuration["ConnectionString"])
|> ignore

services.AddTransient<IFileStorage, FileStorage>()
|> ignore

services.AddControllersWithViews() |> ignore
services.AddHttpContextAccessor() |> ignore

services
    .AddAuthentication(fun authenticationOptions ->
        authenticationOptions.DefaultAuthenticateScheme <- CookieAuthenticationDefaults.AuthenticationScheme
        authenticationOptions.DefaultChallengeScheme <- CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(fun options ->
        options.LoginPath <- "/login"
        options.LogoutPath <- "/logout")
|> ignore

services.AddAuthorization() |> ignore

services.AddScoped<IGotrueSessionPersistence<Session>, CustomSupabaseSessionHandler>()
|> ignore

services.AddScoped<Supabase.Client>(fun sp ->
    let url = builder.Configuration["Supabase:BaseUrl"]

    let key = builder.Configuration["Supabase:SecretApiKey"]

    let sessionHandler = sp.GetRequiredService<IGotrueSessionPersistence<Session>>()

    let options: Supabase.SupabaseOptions =
        Supabase.SupabaseOptions(AutoRefreshToken = true, AutoConnectRealtime = true, SessionHandler = sessionHandler)

    Supabase.Client(url, key, options))
|> ignore

services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
|> ignore

services.AddScoped<AuthService>() |> ignore

services.Configure<ForwardedHeadersOptions>(fun (options: ForwardedHeadersOptions) ->
    options.ForwardedHeaders <-
        ForwardedHeaders.XForwardedFor
        ||| ForwardedHeaders.XForwardedProto

    options.KnownNetworks.Clear()
    options.KnownProxies.Clear())
|> ignore

let app = builder.Build()

app
    .UseStaticFiles()
    .UseHttpsRedirection()
    .Use(
        Func<HttpContext, RequestDelegate, _>(fun ctx next ->
            (task {
                ctx.Response.Headers.Add("X-Clacks-Overhead", "GNU Terry Pratchett")
                return! next.Invoke ctx
            }
            :> Task))
    )
    .UseAuthentication()
    .UseAuthorization()
|> ignore

// https://github.com/albertwoo/FunBlazorSSRDemo
let funGroup = app.MapGroup("").AddFunBlazor()

funGroup.MapGet(
    "",
    Func<NpgsqlDataSource, _>(fun (dataSource: NpgsqlDataSource) ->
        task {
            // TODO: replace getLatestArticle with only get latest slug
            let! latestArticle = Queries.getLatestArticle dataSource

            match latestArticle with
            | None -> return Results.Redirect("/articles/upsert", false)
            | Some la -> return Results.Redirect($"/article/{App.Utils.getUrl la.Id la.Title}", false)
        })
)
|> ignore

funGroup.MapGet(
    "/article/{slug:regex(^(\d+)_.*$)}",
    Func<NpgsqlDataSource, string, _>(fun (dataSource: NpgsqlDataSource) (slug: string) ->
        task {
            let articleId = slug.Substring(0, slug.IndexOf('_')) |> int
            let! article = Queries.getArticleById dataSource articleId

            return Article.view article
        })
)
|> ignore

funGroup.MapGet(
    "/article/{articleId:int}",
    Func<NpgsqlDataSource, int, _>(fun (dataSource: NpgsqlDataSource) (articleId: int) ->
        task {
            let! article = Queries.getArticleById dataSource articleId

            return Results.Redirect($"/article/{App.Utils.getUrl article.Id article.Title}", true, true)
        })
)
|> ignore

funGroup.MapGet(
    "/articles",
    Func<HttpContext, NpgsqlDataSource, _>(fun (ctx: HttpContext) (dataSource: NpgsqlDataSource) ->
        task {
            let (tagExists, tag) = ctx.Request.Query.TryGetValue "tag"

            let! articles =
                if tagExists then
                    Queries.getArticlesByTag dataSource (tag.ToString())
                else
                    Queries.getPublishedArticles dataSource

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
    Func<HttpContext, NpgsqlDataSource, IFileStorage, _>(fun ctx dataSource fileStore ->
        task {
            let! articles = Queries.getAllArticles dataSource

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
                        let! article =
                            Queries.getArticleById dataSource
                            <| int (id.ToString())

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

            let! images = fileStore.GetImages()

            return Upsert.view upsertDocument (images |> Seq.toList)
        })
)
|> ignore

app.MapPost(
    "/upsert",
    Func<HttpContext, NpgsqlDataSource, IFileStorage, _>
        (fun (ctx: HttpContext) (dataSource: NpgsqlDataSource) (fileStorage: IFileStorage) ->
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
                    do! Queries.insertArticle dataSource parsedDocument document.Tags
                    ctx.Response.Headers.Add("HX-Location", "/")
                else
                    do! Queries.updateArticle dataSource id parsedDocument document.Tags
                    ctx.Response.Headers.Add("HX-Location", $"/article/%s{App.Utils.getUrl id parsedDocument.Title}")

                return Results.Created()
            })
)
|> ignore

app.MapDelete(
    "/article/{articleId:int}",
    Func<HttpContext, NpgsqlDataSource, int, _>
        (fun (ctx: HttpContext) (dataSource: NpgsqlDataSource) (articleId: int) ->
            task {
                do! Queries.deleteArticleById dataSource articleId

                ctx.Response.Headers.Add("HX-Location", "/articles/upsert")

                return Results.Accepted()
            })
)
|> ignore

funGroup.MapGet("/about", Func<_>(fun _ -> About.view ()))
|> ignore

funGroup.MapGet("/partials/taginput", Func<_>(fun _ -> TagInput.simple ""))
|> ignore

let feedResult
    (feedFn: string -> Model.ArticleStub list -> XDocument)
    (feedType: string)
    (ctx: HttpContext)
    dataSource
    =
    task {
        let (tagExists, tag) = ctx.Request.Query.TryGetValue "tag"

        let! articles =
            if tagExists then
                Queries.getArticlesByTag dataSource (tag.ToString())
            else
                Queries.getPublishedArticles dataSource

        let articles =
            articles
            |> Seq.map Articles.getArticleStub
            |> List.ofSeq

        let host = ctx.Request.Host.Value

        let xml = (feedFn host articles).ToString()

        return Results.Text(xml, $"application/{feedType}+xml", Encoding.UTF8)
    }

app.MapGet("/atom", Func<HttpContext, NpgsqlDataSource, _>(feedResult Syndication.syndicationFeed "atom"))
|> ignore

app.MapGet("/rss", Func<HttpContext, NpgsqlDataSource, _>(feedResult Syndication.channelFeed "rss"))
|> ignore

app.MapGet("/status", Func<_>(fun _ -> Results.Text("ok")))
|> ignore


//////////
/// Auth
//////////
///
app.MapGet(
    "/secure",
    Func<AuthService, _>(fun authService ->
        task {
            let! user = authService.GetUser()
            return Results.Text($"is secure? {user.Email}")
        })
)
|> ignore

funGroup.MapGet("/login", Func<_>(fun _ -> Login.view ()))
|> ignore

app.MapPost(
    "/login",
    Func<HttpContext, AuthService, IConfiguration, _>(fun ctx authService config ->
        task {
            let! accessToken = authService.Login(config["AdminEmail"], config["AdminPassword"])
            let! user = authService.GetUser()

            return Results.Text(user.Email)
        })
)
|> ignore

funGroup.MapGet(
    "/logout",
    Func<AuthService, _>(fun authService ->
        task {
            do! authService.Logout()

            return Results.Redirect("/", false)
        })
)
|> ignore

app
    .MapGet(
        "/derp",
        Func<HttpContext, _>(fun ctx ->
            let email =
                ctx.User.Claims
                |> Seq.find (fun c -> c.Type = "email")
                |> fun claim -> claim.Value

            Results.Text(email))
    )
    .RequireAuthorization()
|> ignore


app.Run("https://0.0.0.0:5000")
