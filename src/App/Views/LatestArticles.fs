namespace Hashset.Views

open System
open System.Net

open Model

module LatestArticles =
    open Giraffe.GiraffeViewEngine

    let private articles (content: ArticleStub seq) =
        content
        |> Seq.map (fun c ->
            div [ _class "LatestPosts-entry" ] [
                h3 [ _class "LatestPosts-entryTitle" ] [
                    a [ _href <| sprintf "article/%i" c.Id ] [
                        str c.Title
                    ]
                ]
                h5 [ _class "LatestPosts-entryDate" ] [ str <| c.Date.ToString("dd MMMM, yyyy") ]
                div [ _class "LatestPosts-entryTags" ]
                    (c.Tags
                     |> List.map (fun t ->
                        span [ _class "LatestPosts-entryTag" ] [ a [ _class "LatestPosts-entryTagLink"; _href <| sprintf "articles?tag=%s" t.Name ] [ str t.Name ] ]
                     ))
                div [ _class "LatestPosts-entryDescription" ] [ rawText c.Description ]
            ]
        )

    let view (content: ArticleStub seq) =
        div [ _class "PostContents" ] [
            yield! articles content
        ]
