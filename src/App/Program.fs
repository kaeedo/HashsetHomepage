namespace Hashset

open System
open System.Security.Claims

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

open Giraffe
open Saturn

open DataAccess

module Program =
    let version =
        Reflection.Assembly.GetEntryAssembly().GetName()
            .Version

    let me: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            let conf = ctx.GetService<IConfiguration>()

            (authorizeUser (fun u -> u.HasClaim(ClaimTypes.NameIdentifier, conf.["GithubWriteId"]))
                 (setStatusCode 401 >=> text "Access Denied"))
                next
                ctx


    let loggedIn =
        pipeline {
            requires_authentication (Giraffe.Auth.challenge "GitHub")
            plug me
        }

    let securedRouter =
        router {
            pipe_through loggedIn

            get "" Controller.upsertPage
            post "" Controller.upsert
        }

    let articleRouter id =
        router {
            getf "_%s" (fun _ -> Controller.article id)
            get "" (Controller.articleRedirect id)

            // TODO more better delete page
            delete
                ""
                (router { pipe_through loggedIn }
                 >=> Controller.deleteArticle id)
        }

    let browserRouter =
        router {
            case_insensitive
            not_found_handler Controller.homepage

            get "" Controller.homepage
            get "/version" (text (version.ToString()))
            get "/status" (text "ok")

            get "/articles" Controller.articles

            get "/about" Controller.about

            get
                "/rss"
                (setHttpHeader "Content-Type" "application/rss+xml"
                 >=> Controller.rss)

            get
                "/atom"
                (setHttpHeader "Content-Type" "application/atom+xml"
                 >=> Controller.atom)

            forward "/articles/upsert" securedRouter
            forwardf "/article/%i" articleRouter
        }

    let browser =
        pipeline {
            plug putSecureBrowserHeaders
            plug (enableCors CORS.defaultCORSConfig)
            set_header "X-Clacks-Overhead" "GNU Terry Pratchett"
        }

    let topRouter =
        router {
            pipe_through browser

            forward "" browserRouter
        }

    let configureApp (app: IApplicationBuilder) =
        let app =
            app
                .UseStaticFiles()
                .UseAuthentication()
                .UseHttpsRedirection()

        let env = Environment.getWebHostEnvironment app
        if (env.IsDevelopment()) then app.UseDeveloperExceptionPage() else app


    let configureAppConfiguration (config: IConfigurationBuilder) =
        config
            .AddEnvironmentVariables()
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

        services.Configure<ForwardedHeadersOptions>(fun (options: ForwardedHeadersOptions) ->
            options.ForwardedHeaders <-
                ForwardedHeaders.XForwardedFor
                ||| ForwardedHeaders.XForwardedProto

            options.KnownNetworks.Clear()
            options.KnownProxies.Clear())
        |> ignore

        services

    let configureLogging (builder: ILoggingBuilder) =
        let filter (l: LogLevel) = l.Equals LogLevel.Error

        builder.AddFilter(filter).AddConsole().AddDebug()
        |> ignore

    [<EntryPoint>]
    let main _ =
        let app =
            application {
                use_router topRouter
                url "https://0.0.0.0:5000/"
                use_static "WebRoot"
                use_gzip

                use_github_oauth
                    (Environment.GetEnvironmentVariable("GithubClientId"))
                    (Environment.GetEnvironmentVariable("GithubClientSecret"))
                    "/signin-github"
                    [ (ClaimTypes.NameIdentifier, "id") ]

                app_config configureApp

                service_config configureServices
            }
            
        run (app.ConfigureAppConfiguration(Action<IConfigurationBuilder> configureAppConfiguration))

        0
