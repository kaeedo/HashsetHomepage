namespace Hashset

open System
open System.Security.Claims
open System.Configuration
open System.IO

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

open Giraffe

open Hashset.Views
open DataAccess
#if DEBUG
open HttpsConfig
#endif

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
                routeCif "/articles/upsert/%i" (fun id -> mustBeLoggedIn >=> mustBeMe >=> Controller.upsert id)
                routeCif "/article/%i" Controller.article
                routeCi "/about" >=> Controller.about
                routeCi "/rss" >=> setHttpHeader "Content-Type" "application/rss+xml" >=> Controller.rss ]
            POST >=> mustBeLoggedIn >=> mustBeMe >=> choose [
                routeCi "/add" >=> Controller.add >=> redirectTo false "/"
                routeCi "/edit" >=> Controller.edit
            ]
            DELETE >=> mustBeLoggedIn >=> mustBeMe >=> choose [
                routeCif "/article/%i" (fun id -> Controller.deleteArticle id)
            ]
        ]

    let configureApp (app: IApplicationBuilder) =
        app.UseStaticFiles()
           //.UseDeveloperExceptionPage()
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

#if DEBUG
        services.Configure<ForwardedHeadersOptions>(fun (options: ForwardedHeadersOptions) ->
            options.ForwardedHeaders <- ForwardedHeaders.XForwardedFor ||| ForwardedHeaders.XForwardedProto

            options.KnownNetworks.Clear()
            options.KnownProxies.Clear()
        ) |> ignore
#endif

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
            .UseKestrel(
#if DEBUG
                fun o ->
                    o.ConfigureEndpoints [
                        EndpointConfiguration.Default
                        { EndpointConfiguration.Default with
                            Port      = Some 44340
                            Scheme    = Https
                            FilePath  = Some @"../../../../../devCert.pfx"
                            //Password  = None } ]
                            Password = Some (File.ReadAllText(@"..\..\..\..\..\devCert.txt").Trim()) } ]
#endif
            )
            .UseContentRoot(contentRoot)
            .UseWebRoot(webRoot)
            .UseUrls("http://0.0.0.0:5000")
            .ConfigureAppConfiguration(configureAppConfiguration)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()

        0
