namespace Hashset.Views

open Model

module Upsert =
    open Giraffe.GiraffeViewEngine

    let view (upsertDocument: UpsertDocument) =

        let idDropdown =
            option [ _value "0" ] [ str "0" ] ::
            (upsertDocument.ExistingIds
            |> Seq.sortByDescending (fun (id, _) -> id)
            |> Seq.map (fun (id, title) ->
                let attrs =
                    if title = upsertDocument.Title
                    then _selected :: [ _value (id.ToString()) ]
                    else [ _value (id.ToString()) ]
                option attrs [ str title ]
            )
            |> Seq.toList)

        div [] [
            link [ _rel "stylesheet"; _type "text/css"; _href "/css/upsert.css"; _async ]
            form [ _action "/upsert"; _method "POST"; _class "PostContents" ] [
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "Id" ] [ str "Id" ]
                    select [ _class "Upsert-inputValue"; _id "Id"; _name "Id"; ] idDropdown
                ]
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "ArticleDate" ] [ str "ArticleDate" ]
                    input [ _class "Upsert-inputValue"; _type "date"; _id "ArticleDate"; _name "ArticleDate"; _value (upsertDocument.ArticleDate.ToString("yyyy-MM-dd")) ]
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
                    textarea [ _class "Upsert-inputSourceValue"; _rows "10"; _id "Source"; _name "Source" ] [ str upsertDocument.Source ]
                ]
                div [] [
                    div [ _class "Upsert-tagContainer" ] [
                        label [ _class "Upsert-tagLabel" ] [ str "Tags" ]
                        button [ _id "Upsert-addTagButton" ] [ str "+" ]
                    ]
                    ul [ _id "Upsert-tagList" ]
                        (upsertDocument.Tags
                        |> List.map (fun t ->
                            li [ _class "Upsert-tagListItem" ] [
                                div [ _class "Upsert-tagValue" ] [
                                    input [ _type "text"; _name "Tags"; _class "Upsert-tagName"; _value t ]
                                    input [ _type "button"; _class "Upsert-removeTagButton"; _value "-" ]
                                ]
                            ]
                        ))
                ]
                input [ _id "Upsert-submit"; _type "Submit" ]

                script [ _async; _defer; _src "/js/upsert.js"; _type "text/javascript"; ] []
            ]
        ]
