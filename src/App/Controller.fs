namespace Hashset

open FSharp.Control.Tasks.V2.ContextInsensitive
open Hashset.Views
open System
open System.IO
open System.Text.Json
open Giraffe
open Model

module Load =
    let styledMasterView =
        let aboveTheFoldCss = File.ReadAllText("WebRoot/css/aboveTheFold.css")
        Master.view aboveTheFoldCss

[<RequireQualifiedAccess>]
module Controller =
    let private renderArticlePage (article: ParsedDocument) =
        let masterData =
            { MasterContent.PageTitle = article.Title
              ArticleDate = Some article.ArticleDate }

        Article.view article
        |> Load.styledMasterView masterData
        |> htmlView

    let homepage () : HttpHandler  =
        let latestArticle = Articles.getLatestArticle()

        renderArticlePage latestArticle

    let article articleId : HttpHandler =
        renderArticlePage <| Articles.getArticle articleId

    let upsert articleId : HttpHandler =
        let masterData =
            { MasterContent.PageTitle = "Upsert"
              ArticleDate = None }

        let upsertDocument =
            { UpsertDocument.Id = 0
              Title = String.Empty
              Source = String.Empty
              Tags = [] }

        Add.view upsertDocument
        |> Load.styledMasterView masterData
        |> htmlView

    let add : HttpHandler =
        handleContext(
            fun ctx ->
                task {
                    let! document = ctx.BindFormAsync<UpsertDocument>()
                    let! inserted = Articles.parse document.Title document.Source document.Tags
                    return! ctx.WriteTextAsync (inserted.ToString())
                })


    let articles (): HttpHandler =
        // TODO: Server side paging
        let masterData =
            { MasterContent.PageTitle = "All Articles"
              ArticleDate = None }

        let getFirstParagraph (content: string) =
            let firstIndex = content.IndexOf("<p>") + 3
            let lastIndex = content.IndexOf("</p>")
            let count = lastIndex - firstIndex

            content.Substring(firstIndex, count)

        let articles =
            Articles.getArticles()
            |> Seq.map (fun (p: ParsedDocument) ->
                { ArticleStub.Id = p.Id
                  Title = p.Title
                  Date = p.ArticleDate
                  Description = getFirstParagraph p.Document }
            )

        LatestArticles.view articles
        |> Load.styledMasterView masterData
        |> htmlView

    let about (): HttpHandler =
        let masterData =
            { MasterContent.PageTitle = "About Me"
              ArticleDate = None }

        About.view
        |> Load.styledMasterView masterData
        |> htmlView
