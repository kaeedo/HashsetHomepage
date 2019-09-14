namespace Hashset.Views

open System
open System.Net

open Hashset

module LatestPosts =
    open Giraffe.GiraffeViewEngine

    let private posts (content: PostStub seq) =
        content
        |> Seq.map (fun c ->
            div [ _class "LatestPosts-entry" ] [
                h3 [ _class "LatestPosts-entryTitle" ] [
                    a [ _href <| sprintf "posts/%s_%s" (c.Date.ToString("yyyy'-'MM'-'dd")) (WebUtility.UrlEncode(c.Title)) ] [
                        str c.Title
                    ]
                ]
                h5 [ _class "LatestPosts-entryDate" ] [ str <| c.Date.ToString("dd MMMM, yyyy") ]
                div [ _class "LatestPosts-entryDescription" ] [ rawText c.Description ]
            ]
        )

    let view (content: PostStub seq) =
        div [ _class "PostContents" ] [
            yield! posts content
        ]

