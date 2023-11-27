open System

open System.IO
open DataAccess
open Hashset
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
#if !DEBUG
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Hosting
#endif
open Giraffe


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
            routeCi "/" >=> Controller.homepage
            routeCi "/articles" >=> Controller.articles
            routeCi "/articles/upsert"
            >=> mustBeLoggedIn
            >=> Controller.upsertPage
            routeCif "/article/%i_%s" (fun (id, _) -> Controller.article id)
            routeCif "/article/%i" Controller.articleRedirect
            routeCi "/about" >=> Controller.about
            routeCi "/rss"
            >=> setHttpHeader "Content-Type" "application/rss+xml"
            >=> Controller.rss
            routeCi "/atom"
            >=> setHttpHeader "Content-Type" "application/atom+xml"
            >=> Controller.atom
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
    .UseGiraffe(webApp)

app.Run("http://0.0.0.0:5000")
