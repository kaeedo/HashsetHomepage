namespace DataAccess

open System.IO
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive
open System

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> unit
    abstract member GetImages: unit -> string seq


type FileStorage() =
    let path = sprintf "%s/WebRoot/images" Environment.CurrentDirectory

    do Directory.CreateDirectory(path) |> ignore

    interface IFileStorage with
        member this.SaveFile (fileName: string) (copyAsyncFn: Stream -> Task) =
            task {
                use fileStream = File.Create(sprintf "%s/%s" path fileName)

                do! copyAsyncFn (fileStream)
            }
            |> ignore

        member this.GetImages() =
            Directory.EnumerateFiles(path)
            |> Seq.map (fun f -> FileInfo(f).Name)
