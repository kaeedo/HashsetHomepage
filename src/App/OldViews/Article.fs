namespace Hashset.OldViews

open Model

module Article =
    open Giraffe.ViewEngine

    let private _property = attr "property"

    let view (parsedDocument: ParsedDocument) currentUrl =
        let permaLink =
            $"https://%s{currentUrl}/article/%s{App.Utils.getUrl parsedDocument.Id parsedDocument.Title}"

        div [] [
            meta [
                _property "og:site_name"
                _content "Hashset - Kai Ito"
            ]
            meta [
                _property "og:title"
                _content parsedDocument.Title
            ]
            meta [
                _property "og:type"
                _content "article"
            ]
            meta [
                _property "og:description"
                _content parsedDocument.Description
            ]
            meta [
                _property "og:url"
                _content permaLink
            ]
            meta [
                _name "description"
                _content parsedDocument.Description
            ]
            link [ _rel "canonical"; _href <| permaLink ]

            article [ _class "PostContents" ] [
                Text parsedDocument.Document
                Text parsedDocument.Tooltips
            ]
        ]
