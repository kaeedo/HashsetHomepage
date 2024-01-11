namespace App

open System.IO
open System.Threading.Tasks
open System
open Microsoft.Extensions.Configuration

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> unit
    abstract member GetImages: unit -> Task<string seq>

type FileStorage(config: IConfiguration) =
    let path = $"%s{Environment.CurrentDirectory}/WebRoot/images"

    do Directory.CreateDirectory(path) |> ignore
    let url = config["Supabase:BaseUrl"]

    let key = config["Supabase:SecretApiKey"]

    let supabaseClient = Supabase.Client(url, key)

    interface IFileStorage with
        member this.SaveFile (fileName: string) (copyAsyncFn: Stream -> Task) =
            task {
                let savePath = $"%s{path}/%s{fileName}"
                use fileStream = File.Create savePath

                do! copyAsyncFn (fileStream)
            }
            |> ignore

        member this.GetImages() =
            task {
                let! _ = supabaseClient.InitializeAsync()

                let! objects = supabaseClient.Storage.From("images").List()

                return objects |> Seq.map (fun o -> o.Name)
            }
