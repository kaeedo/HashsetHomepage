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
            route "/" >=> warbler (fun _ -> Controller.homepage()) ]

    let configureApp (app : IApplicationBuilder) =
        app.UseStaticFiles()
           .UseGiraffe(webApp)

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        let contentRoot = Directory.GetCurrentDirectory()
        let webRoot     = Path.Combine(contentRoot, "WebRoot")

        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRoot)
            .UseWebRoot(webRoot)
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .Build()
            .Run()
        0

    (* [<EntryPoint>]
    let main _ =
        Reader.write() |> ignore
        0 *)

// https://codeasashu.github.io/hcz-jekyll-blog/jekyll/2016/06/04/welcome-to-jekyll.html
(*
    8BA1A8
    7A787A
    497DCE
    232742
    F0F0F1
*)

// Bookman
// Palatino
// Georgia
