namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown

module Reader =
    let a = 1
    let private getContent key parsed =
        parsed
        |> List.find (fun (span, _) ->
            span = key
        )
        |> fun (_, value) -> value


    let write =
        let listy = File.ReadAllText("./Power.md")

        let parsed = Literate.ProcessMarkdown("./Power.md", generateAnchors = true)

        let pageTitle =
            parsed.Parameters
            |> getContent "page-title"

        let content =
            parsed.Parameters
            |> getContent "document"

        let docOl = Literate.ParseMarkdownString(listy)
        let htmlString = Literate.WriteHtml(docOl)

        //Text htmlString
        parsed.ContentTag.ToString()
