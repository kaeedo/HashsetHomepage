namespace Hashset

open FSharp.Literate
open System.IO
open System
open System.Reflection
open Model
open DataAccess
open Npgsql

[<RequireQualifiedAccess>]
module Articles =
    open DataAccess.Hydra
    let (++) a b = Path.Combine(a, b)

    let private getContent key parsed =
        parsed
        |> List.find (fun (span, _) -> span = key)
        |> fun (_, value) -> value

    // UGLY HACK
    // TODO: Figure out how to do this using FSharp.Literate
    let private transformHtml (document: string) =
        let tableStartTag = "<table class=\"pre\">"
        let tableEndTag = "</table>"

        let rec buildDocument (accumulator: string) (markupToParse: string) =
            let startTableIndex = markupToParse.IndexOf(tableStartTag)

            let endTableIndex =
                markupToParse.IndexOf(tableEndTag)
                + tableEndTag.Length

            let untilTable = markupToParse.Substring(0, startTableIndex)

            let table =
                markupToParse.Substring(startTableIndex, endTableIndex - startTableIndex)

            let afterTable = markupToParse.Substring(endTableIndex)

            let surrounded =
                sprintf "%s%s<div class=\"CodeBlock\">%s</div>" accumulator untilTable table

            if afterTable.Contains(tableStartTag) then
                buildDocument surrounded afterTable
            else
                surrounded + afterTable

        if document.Contains(tableStartTag) then
            buildDocument String.Empty document
        else
            document

    let getLatestArticle (dataSource: NpgsqlDataSource) = Queries.getLatestArticle dataSource
    let getArticles (dataSource: NpgsqlDataSource) = Queries.getArticles dataSource
    let getArticlesByTag (dataSource: NpgsqlDataSource) (tag: string) = Queries.getArticlesByTag dataSource tag
    let getArticle (dataSource: NpgsqlDataSource) (articleId: int) = Queries.getArticleById dataSource articleId
    let deleteArticleById (dataSource: NpgsqlDataSource) (articleId: int) = Queries.deleteArticleById dataSource articleId

    // TODO refactor these
    let addArticle (dataSource: NpgsqlDataSource) (parsedDocument: ParsedDocument) (tags: string list) =
        Queries.insertArticle dataSource parsedDocument tags

    let updateArticle (dataSource: NpgsqlDataSource) (articleId: int) (parsedDocument: ParsedDocument) (tags: string list) =
        Queries.updateArticle dataSource articleId parsedDocument tags

    let parse (document: UpsertDocument) =
        let source = document.Source
        let title = document.Title
        let articleDate = document.ArticleDate

        task {
            let tmpFileName = Path.GetTempPath() ++ Guid.NewGuid().ToString()

            do! File.WriteAllTextAsync(tmpFileName, source)

            let parsed = Literate.ProcessMarkdown(tmpFileName, generateAnchors = true)

            let title = title.Trim()

            let parsedDocument = {
                ParsedDocument.Id = Unchecked.defaultof<int>
                Title = title
                Source = source
                Description = document.Description
                Document =
                    parsed.Parameters
                    |> getContent "document"
                    |> transformHtml
                ArticleDate = articleDate
                Tooltips = parsed.Parameters |> getContent "tooltips"
                Tags = []
            }

            do File.Delete(tmpFileName)

            return parsedDocument
        }

    let getArticleStub (parsedDocument: ParsedDocument) = {
        ArticleStub.Id = parsedDocument.Id
        Title = parsedDocument.Title.Trim()
        Date = parsedDocument.ArticleDate
        Description = parsedDocument.Description
        Excerpt = parsedDocument.GetFirstParagraph
        Tags = parsedDocument.Tags
    }
