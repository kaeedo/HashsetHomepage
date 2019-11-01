namespace Hashset.Views

open Model

module Article =
    open Giraffe.GiraffeViewEngine

    let view (parsedDocument: ParsedDocument) =
        article [ _class "PostContents" ] [
            Text parsedDocument.Document
            Text parsedDocument.Tooltips
        ]
