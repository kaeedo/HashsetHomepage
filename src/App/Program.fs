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
                routeCi "/" >=> warbler (fun _ -> Controller.homepage())
                routeCi "/articles" >=> warbler (fun _ -> Controller.articles())
                routeCi "/articles/upsert" >=> text "wef" //Controller.upsert
                routeCif "/article/%i" Controller.article
                routeCi "/about" >=> Controller.about() ]
            POST >=> choose [
                routeCi "/add" >=> text "wwwavv"  //Controller.add
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

        //Articles.parse @"C:\Users\kait\dev\Hashset\src\App\posts\source\2018-11-11_PowerOfActivePatterns.md" "2018-11-11" ["F#"] |> ignore
        //Articles.parse @"C:\Users\kait\dev\Hashset\src\App\posts\source\2018-11-17_UserDefinedFunction.md" "2018-11-17" ["F#"; "SQLite"] |> ignore

        // let a = Repository.getLatestArticle ()
        // let b = Repository.getArticles ()
        // let c = Repository.getArticleById 1
        // let d = Repository.getArticleById 2

        //https://github.com/giraffe-fsharp/Giraffe
        //https://devblogs.microsoft.com/dotnet/build-a-web-service-with-f-and-net-core-2-0/

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
