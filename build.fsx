#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.Core.TargetOperators

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

"Clean"
    ==> "Build"

// start build
Target.runOrDefault "Build"
