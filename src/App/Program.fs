namespace Hashset

open System

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Giraffe

open Microsoft.Extensions.FileProviders
open System.IO
open Microsoft.AspNetCore.Http

open Hashset.Views

open DataAccess

module App =
    let webApp =
        choose [
            GET >=> choose [
                routeCi "/" >=> Controller.homepage
                routeCi "/articles" >=> Controller.articles
                routeCif "/articles/upsert/%i" Controller.upsert
                routeCif "/article/%i" Controller.article
                routeCi "/about" >=> Controller.about ]
            POST >=> choose [
                routeCi "/add" >=> Controller.add >=> Controller.homepage
                routeCi "/edit" >=> text "jjj" //Controller.edit
            ]
        ]

    let configureApp (app : IApplicationBuilder) =
        app.UseStaticFiles()
           .UseGiraffe(webApp)

    let configureServices (services : IServiceCollection) =
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
            .UseUrls("http://0.0.0.0:5000")
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .Build()
            .Run()

        0
