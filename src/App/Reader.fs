namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown
open System

module Reader =
    let (++) a b = Path.Combine(a,b)

    let private getContent key parsed =
        parsed
        |> List.find (fun (span, _) ->
            span = key
        )
        |> fun (_, value) -> value


    let write =
        let personalDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        let srcDir = personalDir ++ "hashset" ++ "source"
        let parsedDir = personalDir ++ "hashset" ++ "parsed"

        let parsed = Literate.ProcessMarkdown(srcDir ++ "./Power.md", generateAnchors = true)

        let pageTitle =
            parsed.Parameters
            |> getContent "page-title"

        let content =
            parsed.Parameters
            |> getContent "document"

        //Text htmlString
        parsed.ContentTag.ToString()
