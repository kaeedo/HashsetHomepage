namespace Hashset.Views

open Model

module Upsert =
    open Giraffe.GiraffeViewEngine

    let view (upsertDocument: UpsertDocument) =
        let action = if upsertDocument.Id = 0 then "/add" else "/edit"
        form [ _action action; _method "POST"; _class "PostContents" ] [
            div [] [
                label [ _for "Id" ] [ str "Id" ]
                input [ _type "text"; _id "Id"; _name "Id"; _value (upsertDocument.Id.ToString()) ]
            ]
            div [] [
                label [ _for "ArticleDate" ] [ str "ArticleDate" ]
                input [ _type "text"; _id "ArticleDate"; _name "ArticleDate"; _value (upsertDocument.ArticleDate.ToString("d")) ]
            ]
            div [] [
                label [ _for "Title" ] [ str "Title" ]
                input [ _type "text"; _id "Title"; _name "Title"; _value upsertDocument.Title ]
            ]
            div [] [
                label [ _for "Description" ] [ str "Description" ]
                input [ _type "text"; _id "Description"; _name "Description"; _value upsertDocument.Description ]
            ]
            div [] [
                label [ _for "Source" ] [ str "Source" ]
                textarea [ _rows "40"; _cols "160"; _id "Source"; _name "Source" ] [ str upsertDocument.Source ]
            ]
            div [] [
                label [ _id "TagLabel" ] [ str "Tags" ]
                ul [ _id "TagList" ]
                    (upsertDocument.Tags
                    |> List.map (fun t ->
                        li [] [
                            input [ _type "text"; _name "Tags"; _value t ]
                        ]
                    ))
            ]
            input [ _type "Submit" ]

            script [ _async; _defer; _src "/js/upsert.js"; _type "text/javascript"; ] []
        ]
