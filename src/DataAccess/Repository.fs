namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Plans
open Rezoom.SQL.Migrations
open Model

[<RequireQualifiedAccess>]
module Repository =
    let migrate () =
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        Queries.HashsetModel.Migrate(config)

    let private getTag (name: string) =
        plan {
            let! persistedTag =
                Queries.GetTag.Command(name = name).TryExactlyOne()

            match persistedTag with
            | None ->
                let! insertedTag = Queries.InsertTag.Command(name = name).Plan()
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

                let document = { document with Tags = tags }

                let! article =
                    Queries.InsertArticle.Command(
                        title = document.Title,
                        source = document.Source,
                        parsed = document.Document,
                        tooltips = document.Tooltips,
                        createdOn = document.ArticleDate
                        )
                        .Plan()

                let tagIds = tags |> List.map (fun tag -> tag.Id)

                for tagId in batch tagIds do
                    do! Queries.InsertArticleTagsMapping.Command(articleId = article.Id, tagId = tagId).Plan()
            }

        let config = Execution.ExecutionConfig.Default
        (Execution.execute config insertPlan).Result // execute returns Task<'a>
        // TODO: Use TaskBuilder

    let getArticleById id =
        let getPlan =
            plan {
                let! article = Queries.GetArticleById.Command(id = id).ExactlyOne()

                return Queries.mapArticle article
            }

        let config = Execution.ExecutionConfig.Default
        (Execution.execute config getPlan).Result
        // TODO: Use TaskBuilder

    let getArticles () =
        let getPlan =
            plan {
                let! articles = Queries.GetArticles.Command().Plan()

                return
                    articles
                    |> Seq.map (Queries.mapArticle)
            }

        let config = Execution.ExecutionConfig.Default
        (Execution.execute config getPlan).Result
        // TODO: Use TaskBuilder
