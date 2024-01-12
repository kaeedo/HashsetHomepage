namespace App

open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Configuration

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> unit
    abstract member GetImages: unit -> Task<string seq>

type FileStorage(config: IConfiguration) =
    let url = config["Supabase:BaseUrl"]

    let key = config["Supabase:SecretApiKey"]

    let supabaseClient = Supabase.Client(url, key)

    interface IFileStorage with
        member this.SaveFile (fileName: string) (copyAsyncFn: Stream -> Task) =
            task {
                use memoryStream = new MemoryStream()

                do! copyAsyncFn memoryStream

                let! _ =
                    supabaseClient.Storage
                        .From("images")
                        .Upload(memoryStream.ToArray(), fileName)

                return ()
            }
            |> ignore

        member this.GetImages() =
            task {
                let! _ = supabaseClient.InitializeAsync()

                let! objects = supabaseClient.Storage.From("images").List()

                return
                    objects
                    |> Seq.map (fun o ->
                        supabaseClient.Storage
                            .From("images")
                            .GetPublicUrl($"{o.Name}"))
            }
