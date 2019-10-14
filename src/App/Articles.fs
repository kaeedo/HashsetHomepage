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

[<RequireQualifiedAccess>]
module Articles =
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

    let getLatestArticle() =
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

    let getArticle (articleName: string) =
        let articleFileName = sprintf "%s.json" <| articleName.Replace("+", String.Empty).ToLowerInvariant()

        let file =
            DirectoryInfo(parsedDir).GetFiles()
            |> Seq.find(fun f -> f.Name.ToLowerInvariant().Contains(articleFileName))

        let fileContents =
            use fs = file.OpenText()
            fs.ReadToEnd()

        JsonSerializer.Deserialize<ParsedDocument>(fileContents)

    let getArticles() =
        let latestFiles =
            DirectoryInfo(parsedDir).GetFiles()
            |> Seq.sortByDescending (fun f -> f.Name.Split('_').[0])

        latestFiles

    // UGLY HACK
    // TODO: Figure out how to do this using FSharp.Literate
    let transformHtml (document: string) =
        let tableStartTag = "<table class=\"pre\">"
        let tableEndTag = "</table>"
        let divStartTag = "<div class=\"CodeBlock\">"
        let divEndTag = "</div>"

        let rec buildDocument (accumulator: string) (markupToParse: string) =
            let startTableIndex = markupToParse.IndexOf(tableStartTag)
            let endTableIndex = markupToParse.IndexOf(tableEndTag) + tableEndTag.Length

            let untilTable = markupToParse.Substring(0, startTableIndex)
            let table = markupToParse.Substring(startTableIndex, endTableIndex - startTableIndex)
            let afterTable = markupToParse.Substring(endTableIndex)

            let surrounded = sprintf "%s%s<div class=\"CodeBlock\">%s</div>" accumulator untilTable table

            if afterTable.Contains(tableStartTag)
            then buildDocument surrounded afterTable
            else surrounded + afterTable

        if document.Contains(tableStartTag)
        then buildDocument String.Empty document
        else document

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
                  Document = parsed.Parameters |> getContent "document" |> transformHtml
                  ArticleDate = createdDate
                  Tooltips = parsed.Parameters |> getContent "tooltips" }

            let json = JsonSerializer.Serialize(parsedDocument)

            json, sprintf "%s_%s.json" (createdDate.ToString("yyyy'-'MM'-'dd")) title
        )
        |> Seq.toList
        |> List.iter (fun (json, fileName) ->
            File.WriteAllText(parsedDir ++ fileName, json)
        )
