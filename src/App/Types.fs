namespace Model

open System

type Tag = { Id: int; Name: string }

type MasterContent = {
    PageTitle: string
    ArticleDate: DateTime option
    Tags: Tag list
}

type ArticleStub = {
    Id: int
    Title: string
    Date: DateTime
    Description: string
    Excerpt: string
    Tags: Tag list
}

type ParsedDocument = {
    Id: int
    Title: string
    Description: string
    ArticleDate: DateTime
    Source: string
    Document: string
    Tags: Tag list
} with

    member this.GetFirstParagraph =
        let content = this.Document
        let firstIndex = content.IndexOf("<p>") + 3
        let lastIndex = content.IndexOf("</p>")
        let count = lastIndex - firstIndex

        content.Substring(firstIndex, count)

type UpsertDocument = {
    ExistingIds: (int * string) seq
    Title: string
    Description: string
    ArticleDate: DateTime
    Source: string
    Tags: string list
}
