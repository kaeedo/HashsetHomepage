namespace Model

open System

type MasterContent =
    { PageTitle: string
      ArticleDate: DateTime option }

type ArticleStub =
    { Title: string
      Date: DateTime
      Description: string }

type Tag =
    { Id: int
      Name: string}

[<CLIMutable>]
type ParsedDocument =
    { Id: int
      Title: string
      ArticleDate: DateTime
      Source: string
      Document: string
      Tooltips: string
      Tags: Tag list }
