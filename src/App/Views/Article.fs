namespace Hashset.Views

open Model

module Article =
    open Giraffe.GiraffeViewEngine

    let private _property = attr "property"

    let view (parsedDocument: ParsedDocument) currentUrl =
        let permaLink = sprintf "https://%s/article/%s" currentUrl (Utils.getUrl parsedDocument.Id parsedDocument.Title)
        div [] [
            meta [ _property "og:title";  _content parsedDocument.Title ]
            meta [ _property "og:type";  _content "article" ]
            meta [ _property "og:description";  _content parsedDocument.Description ]
            meta [ _property "og:url";  _content permaLink ]
            meta [ _name "description";  _content parsedDocument.Description ]
            link [ _rel "canonical"; _href <| permaLink ]

            article [ _class "PostContents" ] [
                Text parsedDocument.Document
                Text parsedDocument.Tooltips
            ]

            hr [ _class "ArticleEnd" ]

            div [ _class "ArticleComments" ] [
                div [ _id "commento" ] []
            ]

            script [
                _defer
                _async
                _data "no-fonts" "true"
                _data "page-id" (parsedDocument.Id.ToString())
                _src "https://commento.hashset.dev/js/commento.js"
            ] []
        ]
