namespace Model

open System

type Tag =
    { Id: int
      Name: string }

type MasterContent =
    { PageTitle: string
      ArticleDate: DateTime option
      Tags: Tag list }

type ArticleStub =
    { Id: int
      Title: string
      UrlTitle: string
      Date: DateTime
      Description: string
      Tags: Tag list }

type ParsedDocument =
    { Id: int
      Title: string
      ArticleDate: DateTime
      UrlTitle: string
      Source: string
      Document: string
      Tooltips: string
      Tags: Tag list }

[<CLIMutable>]
type UpsertDocument =
    { Id: int
      Title: string
      ArticleDate: DateTime
      Source: string
      Tags: string list }
