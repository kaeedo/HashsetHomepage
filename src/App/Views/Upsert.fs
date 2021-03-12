namespace Hashset.Views

open Model

module Upsert =
    open Giraffe.ViewEngine

    let view (upsertDocument: UpsertDocument) availableImages =

        let idDropdown =
            option [ _value "0" ] [ str "New" ] ::
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

        div [ _class "Upsert-page" ] [
            link [ _rel "stylesheet"; _type "text/css"; _href "/css/upsert.css"; _async ]
            form [ _action "/upsert"; _method "POST"; _class "PostContents Upsert-form"; _enctype "multipart/form-data" ] [
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
                div [ _class "Upsert-inputRow" ] [
                    label [ _class "Upsert-inputLabel"; _for "Images" ] [ str "Upload Images" ]
                    input [ _class "Upsert-inputValue Upsert-inputImages"; _type "file"; _id "Images"; _name "Images"; _multiple ]
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
            aside [ _class "Upsert-availableImagesAside"]
                (availableImages
                 |> List.map (fun ai ->
                     div [ _class "Upsert-availableImageContainer" ] [
                         label [ _class "Upsert-availableImageLabel" ] [ str ai ]
                         img [ _class "Upsert-availableImage"; _src <| sprintf "/images/%s" ai ]
                     ]
                 ))
        ]
