namespace Hashset

open System

type MasterContent =
    { PageTitle: string
      ArticleDate: DateTime option }

type PostStub =
    { Title: string
      Date: DateTime
      Description: string }

[<CLIMutable>]
type ParsedDocument =
    { Title: string
      ArticleDate: DateTime
      Document: string
      Tooltips: string }