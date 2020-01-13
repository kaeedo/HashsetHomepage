namespace Hashset

open System
open System.Net
open System.Xml.Linq

open Model

[<RequireQualifiedAccess>]
module Syndication =
    let private xn = XName.Get
    let private elem name (value:string) = XElement(xn name, value)
    let private elemAttr name attrName attrVal =
        XElement(xn name, XAttribute(xn attrName, attrVal))

    let channelFeed (host: string) (items : ArticleStub list) =
        let elems =
            items
            |> List.map(fun i ->
                XElement(xn "item",
                    elem "title" (WebUtility.HtmlEncode i.Title),
                    elem "link" <| sprintf "https://%s/article/%s" host (Utils.getUrl i.Id i.Title),
                    elem "guid" <| sprintf "%O" (Guid.NewGuid()),
                    elem "pubDate" (i.Date.ToString("r")),
                    elem "description" (WebUtility.HtmlEncode i.Description)
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

    let syndicationFeed (host: string) (items : ArticleStub list) =
        let elems =
            items
            |> List.map(fun i ->
                XElement(xn "entry",
                    elem "title" i.Title,

                    elemAttr "link" "href" (sprintf "https://%s/article/%s" host (Utils.getUrl i.Id i.Title)),
                    elem "guid" <| sprintf "urn:uuid:%O" (Guid.NewGuid()),

                    elem "updated" (i.Date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK")),
                    elem "summary" i.Description
                ))
        XDocument(
            XDeclaration("1.0", "utf-8", "yes"),
                XElement(XName.Get("feed", "http://www.w3.org/2005/Atom"),
                    XAttribute(xn "xmlns", "http://www.w3.org/2005/Atom"),
                    elem "title" "Hashset",
                    elemAttr "link" "href" (sprintf "https://%s" host),
                    XElement(xn "author", elem "name" "Kai Ito"),
                    elem "subtitle" "Kai Ito's blog mainly about programming",
                    elems
                ) |> box)
