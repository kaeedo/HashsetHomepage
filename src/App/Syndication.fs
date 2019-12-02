namespace Hashset

open System
open System.Xml.Linq
open Model

[<RequireQualifiedAccess>]
module Syndication =
    let channelFeed (items : ArticleStub list) =
        let xn = XName.Get
        let elem name (valu:string) = XElement(xn name, valu)
        let elems =
            items
            |> List.map(fun i ->
                XElement(xn "item",
                    elem "title" (System.Net.WebUtility.HtmlEncode i.Title),
                    elem "link" <| sprintf "https://hashset.dev/article/%i" i.Id,
                    elem "guid" <| sprintf "https://hashset.dev/article/%i" i.Id,
                    elem "pubDate" (i.Date.ToString("r")),
                    elem "description" (System.Net.WebUtility.HtmlEncode i.Description)
                ))
        XDocument(
            XDeclaration("1.0", "utf-8", "yes"),
                XElement(xn "rss",
                    XAttribute(xn "version", "2.0"),
                    elem "title" "Hashset",
                    elem "link" "https://hashset.dev",
                    elem "description" "Kai Ito's blog mainly about programming",
                    elem "language" "en-us",
                    XElement(xn "channel", elems)
                ) |> box)
