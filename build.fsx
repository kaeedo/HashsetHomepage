#r "paket:
nuget YUICompressor.NET
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.ReleaseNotes
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open FSharp.Core
open Fake.Core
open Fake.DotNet
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

let tag = sprintf "kaeedo/hashset:%O" version

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
    ["build"; "."; "-t"; tag]
    |> CreateProcess.fromRawCommand "docker"
    |> Proc.run
    |> ignore

    Trace.logfn "Built docker container with tag: %s" tag
)

Target.create "PushContainer" (fun _ ->
    ["login"]
    |> CreateProcess.fromRawCommand "docker"
    |> Proc.run
    |> ignore

    ["push"; tag]
    |> CreateProcess.fromRawCommand "docker"
    |> Proc.run
    |> ignore
)

"Clean"
    ==> "BuildApplication"
    ==> "Minify"

"SetVersion"
    ==> "BuildContainer"
    ==> "PushContainer"

Target.runOrDefault "PushContainer"
