namespace Hashset

open System
open System.Configuration
open System.IO

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.UserSecrets
open Microsoft.AspNetCore.Authentication

open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe

open Hashset.Views
open DataAccess

module Program =

    let challenge (redirectUri : string) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                do! ctx.ChallengeAsync(
                    "Google",
                        AuthenticationProperties(RedirectUri = redirectUri))
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

    let configureApp (app: IApplicationBuilder) =
        //let env = app.ApplicationServices.GetService<IHostingEnvironment>()
        app.UseStaticFiles()
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
        services.AddAuthentication()
                .AddGoogle("Google", fun opt ->
                    let googleAuthNSection: IConfigurationSection = conf.GetSection("Authentication:Google")

                    opt.ClientId <- googleAuthNSection.["ClientId"]
                    opt.ClientSecret <- googleAuthNSection.["ClientSecret"]
                ) |> ignore
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        Repository.migrate()

        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot = Path.Combine(contentRoot, "WebRoot")

        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRoot)
            .UseWebRoot(webRoot)
            .ConfigureAppConfiguration(configureAppConfiguration)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .UseUrls("http://0.0.0.0:5000")
            .Build()
            .Run()

        0
