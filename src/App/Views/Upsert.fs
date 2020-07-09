namespace Hashset.Views

open Model

module Upsert =
    open Giraffe.GiraffeViewEngine

    let view (upsertDocument: UpsertDocument) =
        let action = if upsertDocument.Id = 0 then "/add" else "/edit"
        div [] [
            link [ _rel "stylesheet"; _type "text/css"; _href "/css/upsert.css"; _async ]
            form [ _action action; _method "POST"; _class "PostContents" ] [
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "Id" ] [ str "Id" ]
                    input [ _class "Upsert-inputValue"; _type "text"; _id "Id"; _name "Id"; _value (upsertDocument.Id.ToString()) ]
                ]
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "ArticleDate" ] [ str "ArticleDate" ]
                    input [ _class "Upsert-inputValue"; _type "date"; _id "ArticleDate"; _name "ArticleDate"; _value (upsertDocument.ArticleDate.ToString("d")) ]
                ]
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "Title" ] [ str "Title" ]
                    input [ _class "Upsert-inputValue"; _type "text"; _id "Title"; _name "Title"; _value upsertDocument.Title ]
                ]
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "Description" ] [ str "Description" ]
                    input [ _class "Upsert-inputValue"; _type "text"; _id "Description"; _name "Description"; _value upsertDocument.Description ]
                ]
                div [] [
                    label [ _class "Upsert-inputSourceLabel"; _for "Source" ] [ str "Source" ]
                    textarea [ _class "Upsert-inputSourceValue"; _rows "40"; _id "Source"; _name "Source" ] [ str upsertDocument.Source ]
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
        ]
