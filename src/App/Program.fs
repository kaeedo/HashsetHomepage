namespace Hashset

open System
open System.IO

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
#if !DEBUG
open Microsoft.Extensions.FileProviders
open Microsoft.Extensions.Hosting
#endif

open Giraffe

open DataAccess
open Model
open Microsoft.AspNetCore.Authentication
open System.Net.Http.Headers
open System.Text
open System.Security.Claims



module Program =
    let mustBeLoggedIn: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            requiresAuthentication (Giraffe.Auth.challenge "BasicAuthentication") next ctx

    let version =
        Reflection.Assembly.GetEntryAssembly().GetName()
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

    let configureApp (app: IApplicationBuilder) =
        app
#if !DEBUG
            .UseWebOptimizer()
#endif
            .UseStaticFiles()
            .UseAuthentication()
            .UseHttpsRedirection()
            .UseGiraffe(webApp)

    let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
        config
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
#if DEBUG
            .AddUserSecrets(
                Reflection.Assembly.GetCallingAssembly()
            )
#endif
        |> ignore

    let configureServices (services: IServiceCollection) =
        let sp = services.BuildServiceProvider()
        let conf = sp.GetService<IConfiguration>()

        let repository =
            Repository(conf.["ConnectionString"]) :> IRepository

        repository.Migrate()

        services.AddTransient<IRepository>(fun _ -> repository)
        |> ignore

        services.AddTransient<IFileStorage, FileStorage>()
        |> ignore

        services.Configure<ForwardedHeadersOptions> (fun (options: ForwardedHeadersOptions) ->
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

    let configureLogging (builder: ILoggingBuilder) =
        let filter (l: LogLevel) = l.Equals LogLevel.Error

#if DEBUG
        builder.AddFilter(filter).AddConsole().AddDebug()
        |> ignore
#endif
        ()


    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot = Path.Combine(contentRoot, "WebRoot")

        WebHostBuilder()
            .UseKestrel()
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
