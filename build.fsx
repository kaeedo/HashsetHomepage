#r "paket:
nuget YUICompressor.NET
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Runtime
nuget Fake.Core.ReleaseNotes
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open FSharp.Core
open Fake.Core
open Fake.DotNet
open Fake.Runtime
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Yahoo.Yui.Compressor

// Properties
let buildDir = "./build/"
let solutionFile = "./Hashset.sln"
let entryProject = "./src/App/App.fsproj"

let version =
    let latestRelease = ReleaseNotes.load("./release-notes.md")
    latestRelease.SemVer

let tag = sprintf "%s/hashset:%O" (Environment.environVar "username") version

Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
)

Target.create "BuildApplication" (fun _ ->
    DotNet.build (fun buildOptions ->
        { buildOptions with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some buildDir }
    ) solutionFile
)

Target.create "Minify" (fun _ ->
    let cssCompressor = CssCompressor()

    !!(buildDir @@ "WebRoot" @@ "**" @@ "*.css")
    |> Seq.iter (fun file ->
        cssCompressor.Compress(File.readAsString(file))
        |> File.replaceContent file
    )

    let jsCompressor = JavaScriptCompressor()

    !!(buildDir @@ "WebRoot" @@ "**" @@ "*.js")
    |> Seq.iter (fun file ->
        jsCompressor.Compress(File.readAsString(file))
        |> File.replaceContent file
    )
)

Target.create "SetVersion" (fun _ ->
    let releaseVersion = version.ToString()
    Xml.pokeInnerText entryProject "/Project/PropertyGroup[1]/Version[1]" releaseVersion
    Xml.saveDoc entryProject (Xml.loadDoc entryProject)
)

Target.create "BuildContainer" (fun _ ->
    let result =
        ["build"; "."; "-t"; tag]
            |> CreateProcess.fromRawCommand "docker"
            |> CreateProcess.redirectOutput
            |> Proc.run

    if result.ExitCode <> 0 then
        printfn "%s" result.Result.Output
        failwithf "FAKE Process exited with %d: %s" result.ExitCode result.Result.Error
)

Target.create "PushContainer" (fun _ ->
    let result =
        ["push"; tag]
        |> CreateProcess.fromRawCommand "docker"
        |> CreateProcess.redirectOutput
        |> Proc.run

    if result.ExitCode <> 0 then
        printfn "%s" result.Result.Output
        failwithf "FAKE Process exited with %d: %s" result.ExitCode result.Result.Error

    let result =
        ["logout"]
        |> CreateProcess.fromRawCommand "docker"
        |> CreateProcess.redirectOutput
        |> Proc.run

    if result.ExitCode <> 0 then
        printfn "%s" result.Result.Output
        failwithf "FAKE Process exited with %d: %s" result.ExitCode result.Result.Error
)

"Clean"
    ==> "BuildApplication"
    ==> "Minify"

"SetVersion"
    ==> "BuildContainer"
    ==> "PushContainer"

Target.runOrDefault "PushContainer"
