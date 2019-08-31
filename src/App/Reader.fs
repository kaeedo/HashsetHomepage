namespace Hashset

open FSharp.Literate
open System.IO
open FSharp.Markdown
open System
open System.Text.Json
open System.Text.Json.Serialization

type ParsedDocument =
    { Title: string
      Document: string
      Tooltips: string }

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

        Directory.CreateDirectory(srcDir) |> ignore
        Directory.CreateDirectory(parsedDir) |> ignore

        let parsed = Literate.ProcessMarkdown(srcDir ++ "./Power.md", generateAnchors = true)

        let parsedDocument = { Title = parsed.Parameters |> getContent "page-title"; Document = parsed.Parameters |> getContent "document"; Tooltips = parsed.Parameters |> getContent "tooltips" }
        use outFile = File.CreateText(parsedDir ++ "power.json")

        outFile.Write(JsonSerializer.Serialize<ParsedDocument>(parsedDocument))
        outFile.Flush()

        //Text htmlString
        parsed.ContentTag.ToString()
