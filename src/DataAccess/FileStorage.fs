namespace DataAccess

open System.IO
open System.Threading.Tasks
open System

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> unit
    abstract member GetImages: unit -> string seq


type FileStorage() =
    let path = $"%s{Environment.CurrentDirectory}/WebRoot/images"

    do Directory.CreateDirectory(path) |> ignore

    interface IFileStorage with
        member this.SaveFile (fileName: string) (copyAsyncFn: Stream -> Task) =
            task {
                let savePath = $"%s{path}/%s{fileName}"
                use fileStream = File.Create savePath

                do! copyAsyncFn (fileStream)
            }
            |> ignore

        member this.GetImages() =
            Directory.EnumerateFiles(path)
            |> Seq.map (fun f -> FileInfo(f).Name)
