open System

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Giraffe

open App.Views
open FSharp.Literate
open System.Text
open Giraffe.GiraffeViewEngine
open Microsoft.Extensions.FileProviders
open System.IO
open Microsoft.AspNetCore.Http

// #353535
// #3c6e71
// #FFF
// #D9D9D9
// #284B63

let getHtml () =
    let listy = """
    ### Formatting demo
    let test = ["one";"two";"three"]
    (*** include-value:test ***)"""

    let docOl = Literate.ParseMarkdownString(listy)
    let htmlString = Literate.WriteHtml(docOl)
    Text htmlString

let webApp =
    choose [
        route "/" >=> (Home.view [] |> htmlView)
        route "/test" >=> ([getHtml ()] |> Home.view |> htmlView)]

let configureApp (app : IApplicationBuilder) =
    app.UseStaticFiles() |> ignore
    //app.UseStaticFiles(StaticFileOptions(RequestPath = PathString("/wwwroot/js"), FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()))) |> ignore
    //app.UseStaticFiles(StaticFileOptions(RequestPath = PathString("/wwwroot/css"), FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()))) |> ignore

    app.UseDirectoryBrowser(DirectoryBrowserOptions
        (
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            RequestPath = PathString("")
        )) |> ignore

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
