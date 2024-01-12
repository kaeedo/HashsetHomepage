namespace App

open System.IO
open System.Threading.Tasks

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> unit
    abstract member GetImages: unit -> Task<string seq>

type FileStorage(supabaseClient: Supabase.Client) =
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
