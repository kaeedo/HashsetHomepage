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
            routeCi "/" >=> warbler (fun _ -> Controller.homepage())
            routeCi "/articles" >=> warbler (fun _ -> Controller.articles())
            routeCif "/articles/%s" Controller.article
            routeCi "/about" >=> Controller.about() ]

    let configureApp (app : IApplicationBuilder) =
        app.UseStaticFiles()
           .UseGiraffe(webApp)

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        Repository.migrate()

        Articles.parse @"C:\Users\kait\dev\Hashset\src\App\posts\source\2018-11-11_PowerOfActivePatterns.md" "2018-11-11" ["F#"] |> ignore
        Articles.parse @"C:\Users\kait\dev\Hashset\src\App\posts\source\2018-11-17_UserDefinedFunction.md" "2018-11-17" ["F#"; "SQLite"] |> ignore

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
