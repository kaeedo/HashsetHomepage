namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown
open System
open System.Text.RegularExpressions
open System.Text.Json
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
            |> Array.sortByDescending (fun f ->
                let createdDate = f.Name.Substring(0, 10)
                DateTime.Parse(createdDate)
            )
            |> Array.head

        let fileContents =
            use fs = latest.OpenText()
            fs.ReadToEnd()

        JsonSerializer.Deserialize<ParsedDocument>(fileContents)

    let write() =
        Directory.CreateDirectory(srcDir) |> ignore
        Directory.CreateDirectory(parsedDir) |> ignore

        let allFiles = DirectoryInfo(srcDir).GetFiles()

        allFiles
        |> Seq.map (fun af ->
            let parsed = Literate.ProcessMarkdown(af.ToString(), generateAnchors = true)

            let title = parsed.Parameters |> getContent "page-title"
            let title = Regex.Replace(title, @"\s+", String.Empty)

            let createdDate = af.LastWriteTimeUtc

            let parsedDocument =
                { ParsedDocument.Title = parsed.Parameters |> getContent "page-title"
                  Document = parsed.Parameters |> getContent "document"
                  Tooltips = parsed.Parameters |> getContent "tooltips" }

            let json = JsonSerializer.Serialize(parsedDocument)

            json, sprintf "%s_%s.json" (createdDate.ToString("yyyy'-'MM'-'dd")) title
        )
        |> Seq.iter (fun (json, fileName) ->
            File.WriteAllText(parsedDir ++ fileName, json)
        )
