namespace Hashset

open System
open System.Security.Claims
open System.IO

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

open Giraffe

open DataAccess

module Program =
    let mustBeLoggedIn: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            requiresAuthentication (challenge "GitHub") next ctx

    let mustBeMe: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let conf = ctx.GetService<IConfiguration>()
            (authorizeUser (fun u ->
                u.HasClaim (ClaimTypes.Name, conf.["GithubWriteUsername"])
            ) (setStatusCode 401 >=> text "Access Denied")) next ctx

    let version = Reflection.Assembly.GetEntryAssembly().GetName().Version

    let webApp =
        choose [
            GET >=> choose [
                routeCi "/version" >=> text (version.ToString())
                routeCi "/status" >=> text "ok"
                routeCi "/"  >=> Controller.homepage
                routeCi "/articles" >=> Controller.articles
                routeCi "/articles/upsert" >=> mustBeLoggedIn >=> mustBeMe >=> Controller.upsertPage
                routeCif "/article/%i_%s" (fun (id, _) -> Controller.article id)
                routeCif "/article/%i" Controller.articleRedirect
                routeCi "/about" >=> Controller.about
                routeCi "/rss" >=> setHttpHeader "Content-Type" "application/rss+xml" >=> Controller.rss
                routeCi "/atom" >=> setHttpHeader "Content-Type" "application/atom+xml" >=> Controller.atom ]
            POST >=> mustBeLoggedIn >=> mustBeMe >=> choose [
                routeCi "/upsert" >=> Controller.upsert
            ]
            DELETE >=> mustBeLoggedIn >=> mustBeMe >=> choose [
                routeCif "/article/%i" Controller.deleteArticle
            ]
        ]

    let configureApp (app: IApplicationBuilder) =
        app.UseStaticFiles()
           .UseAuthentication()
           .UseHttpsRedirection()
           .UseGiraffe(webApp)

    let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
        config
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
#if DEBUG
            .AddUserSecrets(Reflection.Assembly.GetCallingAssembly())
#endif
            |> ignore

    let configureServices (services: IServiceCollection) =
        let sp  = services.BuildServiceProvider()
        let conf = sp.GetService<IConfiguration>()

        let repository = Repository(conf.["ConnectionString"]) :> IRepository
        repository.Migrate()
        services.AddTransient<IRepository>(fun _ -> repository) |> ignore

        services.Configure<ForwardedHeadersOptions>(fun (options: ForwardedHeadersOptions) ->
            options.ForwardedHeaders <- ForwardedHeaders.XForwardedFor ||| ForwardedHeaders.XForwardedProto

            options.KnownNetworks.Clear()
            options.KnownProxies.Clear()
        ) |> ignore

        services.AddAuthentication(fun options ->
                    options.DefaultAuthenticateScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    options.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    options.DefaultChallengeScheme <- "GitHub"
                )
                .AddCookie()
                .AddGitHub(fun options ->
                    options.ClientId <- conf.["GithubClientId"]
                    options.ClientSecret <- conf.["GithubClientSecret"]
                    options.CallbackPath <- PathString("/signin-github")

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id")
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name")

                    options.AuthorizationEndpoint <- "https://github.com/login/oauth/authorize"
                    options.TokenEndpoint <- "https://github.com/login/oauth/access_token"
                    options.UserInformationEndpoint <- "https://api.github.com/user"
                ) |> ignore
        services.AddGiraffe() |> ignore

    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Error
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot = Path.Combine(contentRoot, "WebRoot")

        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRoot)
            .UseWebRoot(webRoot)
            .ConfigureAppConfiguration(configureAppConfiguration)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()

        0
