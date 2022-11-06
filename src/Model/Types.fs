namespace Model

open System
open FSlugify.SlugGenerator
open System.Threading.Tasks

type Tag = { Id: int; Name: string }

type MasterContent =
    { PageTitle: string
      ArticleDate: DateTimeOffset option
      Tags: Tag list }

type ArticleStub =
    { Id: int
      Title: string
      Date: DateTimeOffset
      Description: string
      Excerpt: string
      Tags: Tag list }

type ParsedDocument =
    { Id: int
      Title: string
      Description: string
      ArticleDate: DateTime
      Source: string
      Document: string
      Tooltips: string
      Tags: Tag list }
    member this.GetFirstParagraph =
        let content = this.Document
        let firstIndex = content.IndexOf("<p>") + 3
        let lastIndex = content.IndexOf("</p>")
        let count = lastIndex - firstIndex

        content.Substring(firstIndex, count)

    member this.GetUrlTitle =
        slugify DefaultSlugGeneratorOptions this.Title

[<CLIMutable>]
type UpsertDocument =
    { ExistingIds: (int * string) seq
      Title: string
      Description: string
      ArticleDate: DateTimeOffset
      Source: string
      Tags: string list }
