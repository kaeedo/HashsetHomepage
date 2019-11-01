namespace Hashset

open FSharp.Markdown
open System.IO
open System
open System.Text.Json
open System.Reflection
open Model
open DataAccess

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

    // UGLY HACK
    // TODO: Figure out how to do this using FSharp.Literate
    let private transformHtml (document: string) =
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

    let getLatestArticle = Repository.getLatestArticle
    let getArticles = Repository.getArticles
    let getArticle (articleId: int) = Repository.getArticleById articleId

    let parse (title: string) (source: string) (tags: string list) =
        let parsed = Markdown.Parse(source)

        let parsedDocument =
            { ParsedDocument.Id = Unchecked.defaultof<int>
              Title = title.Trim()
              Source = source
              Document = Markdown.WriteHtml(parsed) |> transformHtml
              ArticleDate =  DateTime.Now
              Tooltips = "parsed.Parameters |> getContent tooltips"
              Tags = [] }

        Repository.insertArticle parsedDocument tags
