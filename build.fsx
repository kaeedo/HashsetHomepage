#r "paket:
nuget YUICompressor.NET
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Yahoo.Yui.Compressor

// Properties
let buildDir = "./build/"
let solutionFile = "./Homepage.sln"

// Targets
Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
)

Target.create "Build" (fun _ ->
    DotNet.build (fun buildOptions ->
        { buildOptions with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some buildDir }
    ) solutionFile
)

Target.create "Minify" (fun _ ->
    let cssCompressor = CssCompressor()

    let cssFileName = buildDir @@ "WebRoot" @@ "css" @@ "styles.css"
    let cssFileContents = File.readAsString(cssFileName)

    cssCompressor.Compress(cssFileContents)
    |> File.replaceContent cssFileName


    let jsCompressor = JavaScriptCompressor()
    let jsFiles =
        !!(buildDir @@ "WebRoot" @@ "**" @@ "*.js")

    jsFiles
    |> Seq.iter (fun file ->
        jsCompressor.Compress(File.readAsString(file))
        |> File.replaceContent file
    )
)

"Clean"
    ==> "Build"
    ==> "Minify"

// start build
Target.runOrDefault "Minify"
