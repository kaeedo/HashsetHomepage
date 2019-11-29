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
                if not shouldLoadComments then
                    div [ _class "ArticleComments-container" ] [
                        a [ _class "ArticleComments-loadComments"; _href <| sprintf "/article/%i?loadComments=true#commento" parsedDocument.Id ] [ str "Load comments" ]
                    ]
                div [ _id "commento" ] []
            ]

            if shouldLoadComments then
                script [
                    _defer
                    _async
                    _data "no-fonts" "true"
                    _data "css-override" "/css/comments.css"
                    _src "https://commento.hashset.dev/js/commento.js"
                ] []
        ]
