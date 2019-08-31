namespace Hashset.Views

type ParsedDocument =
    { Title: string
      Document: string
      Tooltips: string }

module Home =
    open Giraffe.GiraffeViewEngine

    let private pageTitle = "hashset.dev"

    let view parsedDocument =
        p [] [ Text parsedDocument.Document ]
