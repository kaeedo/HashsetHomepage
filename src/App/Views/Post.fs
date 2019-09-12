namespace Hashset.Views

[<CLIMutable>]
type ParsedDocument =
    { Title: string
      Document: string
      Tooltips: string }

module Home =
    open Giraffe.GiraffeViewEngine

    let view parsedDocument =
        div [ _class "PostContents" ] [
            Text parsedDocument.Document
            Text parsedDocument.Tooltips
        ]