namespace Hashset

open Markdig
open Markdown.ColorCode
open Model

[<RequireQualifiedAccess>]
module Articles =
    let parse (document: UpsertDocument) =
        let source = document.Source
        let title = document.Title
        let articleDate = document.ArticleDate

        task {
            let pipeline =
                MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseColorCode()
                    .Build()

            let html = Markdown.ToHtml(source, pipeline)

            let title = title.Trim()

            let parsedDocument = {
                ParsedDocument.Id = Unchecked.defaultof<int>
                Title = title
                Source = source
                Description = document.Description
                Document = html
                ArticleDate = articleDate
                Tags = []
            }

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
