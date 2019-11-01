namespace Hashset.Views

open Model

module Add =
    open Giraffe.GiraffeViewEngine

    let view (upsertDocument: UpsertDocument) =
        form [ _action "/add"; _method "POST"; _class "PostContents" ] [
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
                textarea [ _id "Source"; _name "Source" ] [ str upsertDocument.Source ]
            ]
            div [] [
                label [] [ str "Tags" ]
                input [ _type "text"; _name "Tags"; _id "Tags"; _value "we" ]
                input [ _type "text"; _name "Tags"; _id "Tags"; _value "wecc" ]
                input [ _type "text"; _name "Tags"; _id "Tags"; _value "waaea" ]
            ]
            input [ _type "Submit" ]
        ]
