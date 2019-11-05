namespace Hashset

open System
open System.Configuration
open System.IO

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.UserSecrets

open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Hashset.Views
open DataAccess
open HttpsConfig

module Program =

    let challenge (redirectUri : string) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                do! ctx.ChallengeAsync("Google", AuthenticationProperties(RedirectUri = redirectUri))
                return! next ctx
            }

    let googleAuth = challenge "/articles"

    let webApp =
        choose [
            GET >=> choose [
                routeCi "/"  >=> Controller.homepage
                routeCi "/login"  >=> googleAuth
                routeCi "/articles" >=> Controller.articles
                routeCif "/articles/upsert/%i" Controller.upsert
                routeCif "/article/%i" Controller.article
                routeCi "/about" >=> Controller.about ]
            POST >=> choose [
                routeCi "/add" >=> Controller.add >=> Controller.homepage
                routeCi "/edit" >=> text "jjj" //Controller.edit
            ]
        ]

    let error (ex : Exception) _ =
        clearResponse >=> setStatusCode 500 >=> text (sprintf "%s\n%s" ex.Message (ex.StackTrace.ToString()))

    let configureApp (app: IApplicationBuilder) =
        //let env = app.ApplicationServices.GetService<IHostingEnvironment>()
        app.UseStaticFiles()
           .UseGiraffeErrorHandler(error)
           .UseAuthentication()
           .UseGiraffe(webApp)

    let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
        config
            //.AddJsonFile("appsettings.json",false,true)
            //.AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName ,true)
            .AddEnvironmentVariables()
            .AddUserSecrets(System.Reflection.Assembly.GetCallingAssembly()) |> ignore

    let configureServices (services: IServiceCollection) =
        let sp  = services.BuildServiceProvider()
        let conf = sp.GetService<IConfiguration>()
        services.AddAuthentication(fun options ->
                    options.DefaultAuthenticateScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    options.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    options.DefaultChallengeScheme <- "Google"
                )
                .AddCookie()
                .AddGoogle("Google", fun opt ->
                    let googleAuthNSection: IConfigurationSection = conf.GetSection("Authentication:Google")
                    opt.CallbackPath <- PathString("/login")
                    opt.ClientId <- googleAuthNSection.["ClientId"]
                    opt.ClientSecret <- googleAuthNSection.["ClientSecret"]
                ) |> ignore
        services.AddGiraffe() |> ignore
//https://www.eelcomulder.nl/2018/06/12/secure-your-giraffe-application-with-an-oauth-provider/
    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Error
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

    [<EntryPoint>]
    let main _ =
        Repository.migrate()

        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot = Path.Combine(contentRoot, "WebRoot")

        let endpoints =
            [
                EndpointConfiguration.Default
                { EndpointConfiguration.Default with
                    Port      = Some 44340
                    Scheme    = Https
                    //StoreName = Some "My"
                    FilePath  = Some ""
                    Password  = None } ]

        WebHostBuilder()
            .UseKestrel(fun o -> o.ConfigureEndpoints endpoints)
            .UseContentRoot(contentRoot)
            .UseWebRoot(webRoot)
            .ConfigureAppConfiguration(configureAppConfiguration)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Build()
            .Run()

        0
