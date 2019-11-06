namespace Hashset

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2.ContextInsensitive
open Hashset.Views
open System
open System.IO
open Giraffe
open Model
open DataAccess

module private Load =
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

    let homepage: HttpHandler  =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! latestArticle = Articles.getLatestArticle()

                return! renderArticlePage latestArticle next ctx
            }

    let article articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! article = Articles.getArticle articleId

                return! renderArticlePage article next ctx
            }

    let upsert articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let masterData =
                    { MasterContent.PageTitle = "Upsert"
                      ArticleDate = None }

                let upsertDocument =
                    { UpsertDocument.Id = 0
                      Title = String.Empty
                      Source = String.Empty
                      Tags = [] }

                let view =
                    Add.view upsertDocument
                    |> Load.styledMasterView masterData
                    |> htmlView

                return! view next ctx
            }

    let add: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! document = ctx.BindFormAsync<UpsertDocument>()
                let! parsedDocument = Articles.parse document.Title document.Source

                do! Repository.insertArticle parsedDocument document.Tags

                return! next ctx
            }

    let articles: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                // TODO: Server side paging
                let masterData =
                    { MasterContent.PageTitle = "All Articles"
                      ArticleDate = None }

                let getFirstParagraph (content: string) =
                    let firstIndex = content.IndexOf("<p>") + 3
                    let lastIndex = content.IndexOf("</p>")
                    let count = lastIndex - firstIndex

                    content.Substring(firstIndex, count)

                let! articles = Articles.getArticles()
                let articles =
                    articles
                    |> Seq.map (fun (p: ParsedDocument) ->
                        { ArticleStub.Id = p.Id
                          Title = p.Title
                          Date = p.ArticleDate
                          Description = getFirstParagraph p.Document }
                    )

                let view =
                    LatestArticles.view articles
                    |> Load.styledMasterView masterData
                    |> htmlView

                return! view next ctx
            }

    let about: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let masterData =
                    { MasterContent.PageTitle = "About Me"
                      ArticleDate = None }

                let view =
                    About.view
                    |> Load.styledMasterView masterData
                    |> htmlView

                return! view next ctx
            }
