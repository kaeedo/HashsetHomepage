namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Plans
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open Model

type internal HashsetModel = SQLModel<".">

type internal InsertTag = SQL<"""
    insert into tags
    row
        Name = @name;
    select lastval() as Id;
""">

type internal GetTag = SQL<"""
    select * from tags
    where Name = @name
""">

type internal InsertArticle = SQL<"""
    insert into articles
    row
        Title = @title,
        Source = @source,
        Parsed = @parsed,
        Tooltips = @tooltips,
        CreatedOn = @createdOn;
    select lastval() as Id;
""">

type internal InsertArticleTagsMapping = SQL<"""
    insert into article_tags
    row
        ArticleId = @articleId,
        TagId = @tagId
""">

[<RequireQualifiedAccess>]
module Repository =
    let migrate () =
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        HashsetModel.Migrate(config)

    let private getTag (name: string) =
        plan {
            let! persistedTag =
                GetTag.Command(name = name).TryExactlyOne()

            match persistedTag with
            | None ->
                let! insertedTag = InsertTag.Command(name = name).Plan()
                return { Tag.Id = insertedTag.Id; Name = name }
            | Some tag ->
                return { Tag.Id = tag.Id; Name = tag.Name }
        }

    let insertArticle (document: ParsedDocument) (tags: string list) =
        let insertPlan =
            plan {
                let! tags =
                    Plan.concurrentList
                        [ for tagName in tags do
                              getTag tagName ]

                let document = { document with Tags = Some tags }

                let! article =
                    InsertArticle.Command(
                        title = document.Title,
                        source = document.Source,
                        parsed = document.Document,
                        tooltips = document.Tooltips,
                        createdOn = document.ArticleDate
                        )
                        .Plan()

                let tagIds = tags |> List.map (fun tag -> tag.Id)

                for tagId in batch tagIds do
                    do! InsertArticleTagsMapping.Command(articleId = article.Id, tagId = tagId).Plan()
            }

        let config = Execution.ExecutionConfig.Default
        (Execution.execute config insertPlan).Result
        // TODO: Use TaskBuilder
