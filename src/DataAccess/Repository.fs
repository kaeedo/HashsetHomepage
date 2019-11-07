namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Plans
open Rezoom.SQL.Migrations
open Rezoom.SQL.Mapping
open Model

[<RequireQualifiedAccess>]
module Repository =
    let private serviceConfig = ServiceConfig()
    serviceConfig.SetConfiguration<ConnectionProvider>(new HashsetConnectionProvider("Host=localhost;Port=5454;Database=hashset;Username=postgres")) |> ignore

    let private executionConfig =
        { Execution.ExecutionConfig.Default with
            ServiceConfig = serviceConfig :> IServiceConfig }

    let migrate () =
        // https://github.com/rspeele/Rezoom.SQL/issues/49
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        let connection = System.Configuration.ConnectionStringSettings()
        connection.ConnectionString <- "Host=localhost;Port=5454;Database=hashset;Username=postgres"
        connection.ProviderName <- "Npgsql"
        Queries.HashsetModel.MigrateWithConnection(config, connection)

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

        Execution.execute executionConfig insertPlan

    let updateArticle (articleId: int) (document: ParsedDocument) (tags: string list) =
        let updatePlan =
            plan {
                let! tags =
                    Plan.concurrentList
                        [ for tagName in tags do
                              getTag tagName ]

                let document = { document with Tags = tags }

                let! article =
                    Queries.UpdateArticleById.Command(
                        id = articleId,
                        title = document.Title,
                        source = document.Source,
                        parsed = document.Document,
                        tooltips = document.Tooltips
                        )
                        .Plan()

                let tagIds = tags |> List.map (fun tag -> tag.Id)

                for tagId in batch tagIds do
                    do! Queries.InsertArticleTagsMapping.Command(articleId = articleId, tagId = tagId).Plan()
            }

        Execution.execute executionConfig updatePlan

    let getArticleById id =
        let getPlan =
            plan {
                let! article =
                    Queries.GetArticleById.Command(id = id).ExactlyOne()

                return Queries.mapArticle article
            }

        Execution.execute executionConfig getPlan

    let deleteArticleById id =
        let deletePlan =
            plan {
                do! Queries.DeleteArticleTagsByArticleId.Command(id = id).Plan()
                do! Queries.DeleteArticleById.Command(id = id).Plan()
            }

        Execution.execute executionConfig deletePlan

    let getLatestArticle () =
        let getPlan =
            plan {
                let! articles = Queries.GetLatestArticle.Command().Plan()

                return Queries.mapArticle (articles.[0])
            }

        Execution.execute executionConfig getPlan

    let getArticles () =
        let getPlan =
            plan {
                let! articles = Queries.GetArticles.Command().Plan()

                return
                    articles
                    |> Seq.map (Queries.mapArticle)
            }

        Execution.execute executionConfig getPlan
