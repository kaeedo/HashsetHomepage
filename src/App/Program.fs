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
    let private homepage =
        let masterData =
            { Author = "Kai Ito"
              JobTitle = "Software Developer"
              PageTitle= "Home"
              ArticleDate = DateTime.Now.ToShortDateString() }

        Home.view
        |> Master.view masterData
        |> htmlView

    let webApp =
        choose [
            route "/" >=> homepage]
            //route "/test" >=> warbler (fun _ -> [getHtml] |> Home.view |> htmlView)]

    let configureApp (app : IApplicationBuilder) =
        app.UseStaticFiles() |> ignore

        app.UseGiraffe webApp

    let configureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    [<EntryPoint>]
    let main _ =
        WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .Build()
            .Run()
        0
    (* [<EntryPoint>]
    let main _ =
        Reader.write |> ignore
         *)0

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
