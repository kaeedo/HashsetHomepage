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
    let private renderArticlePage (shouldLoadComments: bool) (article: ParsedDocument) currentUrl =
        let masterData =
            { MasterContent.PageTitle = article.Title
              ArticleDate = Some article.ArticleDate
              Tags = article.Tags }

        Article.view shouldLoadComments article currentUrl
        |> Load.styledMasterView masterData
        |> htmlView

    let homepage: HttpHandler  =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                let! latestArticle = Articles.getLatestArticle repository

                let host = ctx.Request.Host.Value

                return! renderArticlePage false latestArticle host next ctx
            }

    let articleRedirect articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                let! article = Articles.getArticle repository articleId

                return! redirectTo true (sprintf "/article/%s" (Utils.getUrl article.Id article.Title)) next ctx
            }

    let article articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let shouldLoadComments =
                    match ctx.TryGetQueryStringValue "loadComments" with
                    | None -> false
                    | Some _ -> true

                let repository = ctx.GetService<IRepository>()
                let! article = Articles.getArticle repository articleId

                let host = ctx.Request.Host.Value

                return! renderArticlePage shouldLoadComments article host next ctx
            }

    let deleteArticle articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                do! Articles.deleteArticleById repository articleId

                return! next ctx
            }

    let upsert articleId: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let masterData =
                    { MasterContent.PageTitle = "Upsert"
                      ArticleDate = None
                      Tags = [] }
                let! upsertDocument =
                    if articleId = 0 then
                        task {
                            return { UpsertDocument.Id = articleId
                                     Title = String.Empty
                                     Source = String.Empty
                                     ArticleDate = DateTime.UtcNow.Date
                                     Tags = [] }
                        }
                    else
                        task {
                            let repository = ctx.GetService<IRepository>()
                            let! article = Articles.getArticle repository articleId
                            return { UpsertDocument.Id = articleId
                                     Title = article.Title
                                     Source = article.Source
                                     ArticleDate = article.ArticleDate
                                     Tags = article.Tags |> List.map (fun t -> t.Name) }
                        }

                let view =
                    Upsert.view upsertDocument
                    |> Load.styledMasterView masterData
                    |> htmlView

                return! view next ctx
            }

    let add: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                let! document = ctx.BindFormAsync<UpsertDocument>()
                let! parsedDocument = Articles.parse document.Title document.Source document.ArticleDate

                do! Articles.addArticle repository parsedDocument document.Tags

                return! next ctx
            }

    let edit: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                let! document = ctx.BindFormAsync<UpsertDocument>()
                let! parsedDocument = Articles.parse document.Title document.Source document.ArticleDate

                do! Articles.updateArticle repository document.Id parsedDocument document.Tags

                return! redirectTo false (sprintf "/article/%s" (Utils.getUrl parsedDocument.Id parsedDocument.Title)) next ctx
            }

    let articles: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()
                let masterData =
                    { MasterContent.PageTitle = "All Articles"
                      ArticleDate = None
                      Tags = [] }

                let! articles =
                    match ctx.TryGetQueryStringValue "tag" with
                    | None -> Articles.getArticles repository
                    | Some t -> Articles.getArticlesByTag repository t

                let articles =
                    articles
                    |> Seq.map Articles.getArticleStub

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
                      ArticleDate = None
                      Tags = [] }

                let view =
                    About.view
                    |> Load.styledMasterView masterData
                    |> htmlView

                return! view next ctx
            }

    let rss: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()

                let! articles =
                    match ctx.TryGetQueryStringValue "tag" with
                    | None -> Articles.getArticles repository
                    | Some t -> Articles.getArticlesByTag repository t

                let articles =
                    articles
                    |> Seq.map Articles.getArticleStub
                    |> List.ofSeq

                let host = ctx.Request.Host.Value

                return! ctx.WriteStringAsync <| (Syndication.channelFeed host articles).ToString()
            }

    let atom: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let repository = ctx.GetService<IRepository>()

                let! articles =
                    match ctx.TryGetQueryStringValue "tag" with
                    | None -> Articles.getArticles repository
                    | Some t -> Articles.getArticlesByTag repository t

                let articles =
                    articles
                    |> Seq.map Articles.getArticleStub
                    |> List.ofSeq

                let host = ctx.Request.Host.Value

                return! ctx.WriteStringAsync <| (Syndication.syndicationFeed host articles).ToString()
            }
