namespace Hashset

open FSharp.Literate
open System.IO

module Reader =
    let a = 1

    let write =
        let listy = File.ReadAllText("./Power.md")
        let docOl = Literate.ParseMarkdownString(listy)
        let htmlString = Literate.WriteHtml(docOl)
        //Text htmlString
        ""
