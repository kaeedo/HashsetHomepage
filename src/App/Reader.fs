namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown
open System
open Newtonsoft.Json

open Hashset.Views

module Reader =
    let (++) a b = Path.Combine(a,b)
    let private personalDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
    let private srcDir = personalDir ++ "hashset" ++ "source"
    let private parsedDir = personalDir ++ "hashset" ++ "parsed"

    let private getContent key parsed =
        parsed
        |> List.find (fun (span, _) ->
            span = key
        )
        |> fun (_, value) -> value

    let getLatestPost() =
        let directory = DirectoryInfo(parsedDir)
        let latest =
            directory.GetFiles()
            |> Array.sortByDescending (fun f -> f.LastWriteTimeUtc)
            |> Array.head

        let fileContents =
            use fs = latest.OpenText()
            fs.ReadToEnd()

        // CLIMutable attribute. Remember that. Then maybe System.TextJson will work
        JsonConvert.DeserializeObject<ParsedDocument>(fileContents)

    let write() =
        Directory.CreateDirectory(srcDir) |> ignore
        Directory.CreateDirectory(parsedDir) |> ignore

        let parsed = Literate.ProcessMarkdown(srcDir ++ "./Power.md", generateAnchors = true)

        let parsedDocument = { ParsedDocument.Title = parsed.Parameters |> getContent "page-title"; Document = parsed.Parameters |> getContent "document"; Tooltips = parsed.Parameters |> getContent "tooltips" }

        let json = JsonConvert.SerializeObject(parsedDocument)
        File.WriteAllText(parsedDir ++ "power.json", json)

        //Text htmlString
        parsed.ContentTag.ToString()
