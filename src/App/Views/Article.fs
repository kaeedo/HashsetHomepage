namespace Hashset.Views

open Model

module Article =
    open Giraffe.GiraffeViewEngine

    let view (shouldLoadComments: bool) (parsedDocument: ParsedDocument) =
        div [] [
            article [ _class "PostContents" ] [
                Text parsedDocument.Document
                Text parsedDocument.Tooltips
            ]

            hr [ _class "ArticleEnd" ]

            div [ _class "ArticleComments" ] [
                div [ _class "ArticleComments-container" ] [
                    if not shouldLoadComments then
                        a [ _id "loadComments"; _href <| sprintf "/article/%i?loadComments=true#commento" parsedDocument.Id ] [ str "Load comments" ]
                ]
                div [ _id "commento" ] []
            ]

            script [
                _defer
                _async
                _data "no-fonts" "true"
                _data "css-override" "/css/commentoOverride.css"
                _data "auto-init" (shouldLoadComments.ToString().ToLowerInvariant())
                _src "https://commento.hashset.dev/js/commento.js"
            ] []
            script [ _async; _defer; _src "/js/article.js"; _type "text/javascript"; ] []
        ]
