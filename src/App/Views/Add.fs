namespace Hashset.Views

open Model

module Add =
    open Giraffe.GiraffeViewEngine

    let view (upsertDocument: UpsertDocument) =
        let action = if upsertDocument.Id = 0 then "/add" else "/edit"
        form [ _action action; _method "POST"; _class "PostContents" ] [
            div [] [
                label [ _for "Id" ] [ str "Id" ]
                input [ _type "text"; _id "Id"; _name "Id"; _value (upsertDocument.Id.ToString()) ]
            ]
            div [] [
                label [ _for "Title" ] [ str "Title" ]
                input [ _type "text"; _id "Title"; _name "Title"; _value upsertDocument.Title ]
            ]
            div [] [
                label [ _for "Source" ] [ str "Source" ]
                textarea [ _rows "40"; _cols "160"; _id "Source"; _name "Source" ] [ str upsertDocument.Source ]
            ]
            div [] [
                label [ _id "TagLabel" ] [ str "Tags" ]
                ul [ _id "TagList" ] [
                    li [] [
                        input [ _type "text"; _name "Tags"; ]
                    ]

                ]
            ]
            input [ _type "Submit" ]

            script [ _async; _defer; _src "/js/upsert.js"; _type "text/javascript"; ] []
        ]
