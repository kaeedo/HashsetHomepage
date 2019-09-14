namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown
open FSharp.Formatting.Common
open System
open System.Text.RegularExpressions
open System.Text.Json
open System.Reflection
open Hashset.Views

module Posts =
    let (++) a b = Path.Combine(a, b)
    let private personalDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
    let private srcDir = personalDir ++ "posts" ++ "source"
    let private parsedDir = personalDir ++ "posts" ++ "parsed"

    let private getContent key parsed =
        parsed
        |> List.find (fun (span, _) ->
            span = key
        )
        |> fun (_, value) -> value

    let getLatestPost() =
        let latest =
            DirectoryInfo(parsedDir).GetFiles()
            |> Array.sortByDescending (fun f ->
                let createdDate = f.Name.Substring(0, 10)
                DateTime.Parse(createdDate)
            )
            |> Array.head

        let fileContents =
            use fs = latest.OpenText()
            fs.ReadToEnd()

        JsonSerializer.Deserialize<ParsedDocument>(fileContents)

    let getPost (postName: string) =
        let postFileName = sprintf "%s.json" <| postName.Replace("+", String.Empty).ToLowerInvariant()

        let file =
            DirectoryInfo(parsedDir).GetFiles()
            |> Seq.find(fun f -> f.Name.ToLowerInvariant() = postFileName)

        let fileContents =
            use fs = file.OpenText()
            fs.ReadToEnd()

        JsonSerializer.Deserialize<ParsedDocument>(fileContents)

    let getPosts() =
        let latestFiles =
            DirectoryInfo(parsedDir).GetFiles()
            |> Seq.sortByDescending (fun f -> f.Name.Split('_').[0])

        latestFiles

    let parseAll() =
        Directory.CreateDirectory(srcDir) |> ignore
        Directory.CreateDirectory(parsedDir) |> ignore

        DirectoryInfo(srcDir).GetFiles()
        |> Seq.map (fun f ->
            let parsed = Literate.ProcessMarkdown(f.ToString(), generateAnchors = true)

            let title = parsed.Parameters |> getContent "page-title"
            let title = Regex.Replace(title, @"\s+", String.Empty)

            let createdDate = f.Name.Split('_').[0]
            let createdDate = DateTime.Parse(createdDate)

            let parsedDocument =
                { ParsedDocument.Title = parsed.Parameters |> getContent "page-title"
                  Document = parsed.Parameters |> getContent "document"
                  ArticleDate = createdDate
                  Tooltips = parsed.Parameters |> getContent "tooltips" }

            let json = JsonSerializer.Serialize(parsedDocument)

            json, sprintf "%s_%s.json" (createdDate.ToString("yyyy'-'MM'-'dd")) title
        )
        |> Seq.toList
        |> List.iter (fun (json, fileName) ->
            File.WriteAllText(parsedDir ++ fileName, json)
        )
