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

module App =
    let webApp =
        choose [
            routeCi "/" >=> warbler (fun _ -> Controller.homepage())
            routeCi "/posts" >=> warbler (fun _ -> Controller.posts())
            routeCif "/posts/%s" Controller.post
            routeCi "/about" >=> Controller.about() ]

    let configureApp (app : IApplicationBuilder) =
        app.UseStaticFiles()
           .UseGiraffe(webApp)

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        Posts.parseAll() |> ignore
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
