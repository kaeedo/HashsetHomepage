namespace Hashset.Views

open System

open Model

module LatestArticles =
    open Giraffe.ViewEngine

    let private articles (content: ArticleStub seq) =
        content
        |> Seq.map (fun c ->
            div [ _class "LatestPosts-entry" ] [
                h3 [ _class "LatestPosts-entryTitle" ] [
                    a [
                        _href
                        <| sprintf "article/%s" (App.Utils.getUrl c.Id c.Title)
                    ] [ str c.Title ]
                ]
                h5 [ _class "LatestPosts-entryDate" ] [
                    str <| c.Date.ToString("dd MMMM, yyyy")
                ]
                div
                    [ _class "LatestPosts-entryTags" ]
                    (c.Tags
                     |> List.map (fun t ->
                         let encoded = Web.HttpUtility.UrlEncode(t.Name, Text.Encoding.ASCII)

                         a [
                             _class "LatestPosts-entryTag"
                             _href <| sprintf "articles?tag=%s" encoded
                         ] [ str t.Name ]))
                div [ _class "LatestPosts-entryExcerpt" ] [ rawText c.Excerpt ]
            ])
        |> Seq.toList

    let view (content: ArticleStub seq) =
        div [ _class "LatestArticles" ] (articles content)
