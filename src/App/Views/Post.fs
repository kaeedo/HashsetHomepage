namespace Hashset.Views

open Hashset

module Post =
    open Giraffe.GiraffeViewEngine

    let view parsedDocument =
        div [ _class "PostContents" ] [
            Text parsedDocument.Document
            Text parsedDocument.Tooltips
        ]
