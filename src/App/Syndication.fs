namespace Hashset

open System
open System.Xml.Linq

open Model

[<RequireQualifiedAccess>]
module Syndication =
    let channelFeed (host: string) (items : ArticleStub list) =
        let xn = XName.Get
        let elem name (value:string) = XElement(xn name, value)
        let elems =
            items
            |> List.map(fun i ->
                XElement(xn "item",
                    elem "title" (System.Net.WebUtility.HtmlEncode i.Title),
                    elem "link" <| sprintf "https://%s/article/%i" host i.Id,
                    elem "guid" <| sprintf "https://%s/article/%i" host i.Id,
                    elem "pubDate" (i.Date.ToString("r")),
                    elem "description" (System.Net.WebUtility.HtmlEncode i.Description)
                ))
        XDocument(
            XDeclaration("1.0", "utf-8", "yes"),
                XElement(xn "rss",
                    XAttribute(xn "version", "2.0"),
                    elem "title" "Hashset",
                    elem "link" <| sprintf "https://%s" host,
                    elem "description" "Kai Ito's blog mainly about programming",
                    elem "language" "en-us",
                    XElement(xn "channel", elems)
                ) |> box)

    // let syndicationFeed (host: string) (items : ArticleStub list) =
    //     let xn = XName.Get
    //     let elem name (value:string) = XElement(xn name, value)
    //     let elemAttr name (attr: string) = XElement(xn name, XAttribute)
    //     let elems =
    //         items
    //         |> List.map(fun i ->
    //             XElement(xn "entry",
    //                 elem "title" (System.Net.WebUtility.HtmlEncode i.Title),

    //                 elemAttr "link" <| sprintf "https://%s/article/%i" host i.Id,
    //                 elem "guid" <| sprintf "urn:uuid:%O" Guid.NewGuid(),

    //                 elem "updated" (i.Date.ToString("r")),
    //                 elem "summary" (System.Net.WebUtility.HtmlEncode i.Description)
    //             ))
    //     XDocument(
    //         XDeclaration("1.0", "utf-8", "yes"),
    //             XElement(xn "feed",
    //                 XAttribute(xn "xmlns", "http://www.w3.org/2005/Atom"),
    //                 elem "title" "Hashset",

    //                 elemAttr "link" <| sprintf "https://%s" host,

    //                 elem "subtitle" "Kai Ito's blog mainly about programming",
    //                 elem "language" "en-us",
    //                 XElement(xn "entry", elems)
    //             ) |> box)
