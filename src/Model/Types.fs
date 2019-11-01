namespace Model

open System

type MasterContent =
    { PageTitle: string
      ArticleDate: DateTime option }

type ArticleStub =
    { Id: int
      Title: string
      Date: DateTime
      Description: string }

type Tag =
    { Id: int
      Name: string }

type ParsedDocument =
    { Id: int
      Title: string
      ArticleDate: DateTime
      Source: string
      Document: string
      Tooltips: string
      Tags: Tag list }

[<CLIMutable>]
type UpsertDocument =
    { Id: int
      Title: string
      Source: string
      Tags: string list }
