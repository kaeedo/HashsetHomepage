namespace Hashset.Views

open Model

module Article =
    open Giraffe.GiraffeViewEngine

    let view (parsedDocument: ParsedDocument) =
        article [ _class "PostContents" ] [
            Text parsedDocument.Document
            Text parsedDocument.Tooltips

            button [ _id "getComments" ] [ str "Comments" ]
            div [ _id "commento" ] []
            script [
                _defer
                _async
                _data "no-fonts" "true"
                _data "css-override" "/css/commentoOverride.css"
                _data "auto-init" "false"
                _src "https://commento.hashset.dev/js/commento.js"
            ] []
            script [ _async; _defer; _src "/js/article.js"; _type "text/javascript"; ] []
        ]
