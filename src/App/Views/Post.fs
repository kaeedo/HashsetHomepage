namespace Hashset.Views

[<CLIMutable>]
type ParsedDocument =
    { Title: string
      Document: string
      Tooltips: string }

module Home =
    open Giraffe.GiraffeViewEngine

    let view parsedDocument =
        div [] [
            p [] [ Text parsedDocument.Document ]
            Text parsedDocument.Tooltips
        ]
