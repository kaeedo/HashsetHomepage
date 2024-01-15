namespace App

open System.IO
open System.Threading.Tasks

type IFileStorage =
    abstract member SaveFile: string -> (Stream -> Task) -> Task<unit>
    abstract member GetImages: unit -> Task<string seq>
    abstract member DeleteImage: string -> Task<unit>

type FileStorage(supabaseClient: Supabase.Client) =
    interface IFileStorage with
        member this.SaveFile (fileName: string) (copyAsyncFn: Stream -> Task) =
            task {
                let! _ = supabaseClient.InitializeAsync()
                use memoryStream = new MemoryStream()

                do! copyAsyncFn memoryStream

                let! _ =
                    supabaseClient.Storage
                        .From("images")
                        .Upload(memoryStream.ToArray(), fileName)

                return ()
            }

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

        member this.DeleteImage(image) =
            task {
                let! _ = supabaseClient.InitializeAsync()

                let! _ =
                    supabaseClient.Storage
                        .From("images")
                        .Remove(image)

                return ()
            }
