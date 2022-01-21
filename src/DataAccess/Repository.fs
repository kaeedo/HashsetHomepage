namespace DataAccess

open System
open System.Threading.Tasks

open Rezoom
open Rezoom.SQL.Plans
open Rezoom.SQL.Migrations
open Rezoom.SQL.Mapping
open Npgsql.Logging

open Model

type IRepository =
    abstract member Migrate : unit -> unit
    abstract member InsertArticle : ParsedDocument -> string list -> Task<unit>
    abstract member UpdateArticle : int -> ParsedDocument -> string list -> Task<unit>
    abstract member GetArticleById : int -> Task<ParsedDocument>
    abstract member DeleteArticleById : int -> Task<unit>
    abstract member GetLatestArticle : unit -> Task<ParsedDocument option>
    abstract member GetArticles : unit -> Task<ParsedDocument seq>
    abstract member GetAllArticles : unit -> Task<ParsedDocument seq>
    abstract member GetArticlesByTag : string -> Task<ParsedDocument seq>

type Repository(connectionString) =
#if DEBUG
    do NpgsqlLogManager.Provider <- ConsoleLoggingProvider(NpgsqlLogLevel.Trace, true, true) :> INpgsqlLoggingProvider
    do NpgsqlLogManager.IsParameterLoggingEnabled <- true
#endif

    let serviceConfig = ServiceConfig()

    do
        serviceConfig.SetConfiguration<ConnectionProvider>(HashsetConnectionProvider(connectionString))
        |> ignore

    let executionConfig =
        { Execution.ExecutionConfig.Default with ServiceConfig = serviceConfig :> IServiceConfig }

    let getTag (name: string) =
        plan {
            let! persistedTag =
                Queries
                    .GetTag
                    .Command(name = name)
                    .TryExactlyOne()

            match persistedTag with
            | None ->
                let! insertedTag = Queries.InsertTag.Command(name = name).Plan()
                return { Tag.Id = insertedTag.Id; Name = name }
            | Some tag -> return { Tag.Id = tag.Id; Name = tag.Name }
        }

    interface IRepository with
        member this.Migrate() =
            let config =
                { MigrationConfig.Default with LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

            let connection = Configuration.ConnectionStringSettings()
            connection.ConnectionString <- connectionString
            connection.ProviderName <- "Npgsql"
            Queries.HashsetModel.Migrate(config, connection)

        member this.InsertArticle (document: ParsedDocument) (tags: string list) =
            let insertPlan =
                plan {
                    let! tags =
                        Plan.concurrentList [
                            for tagName in tags do
                                getTag tagName
                        ]

                    let document = { document with Tags = tags }

                    let! article =
                        Queries
                            .InsertArticle
                            .Command(
                                title = document.Title,
                                source = document.Source,
                                description = document.Description,
                                parsed = document.Document,
                                tooltips = document.Tooltips,
                                createdOn = document.ArticleDate
                            )
                            .Plan()

                    let tagIds = tags |> List.map (fun tag -> tag.Id)

                    for tagId in batch tagIds do
                        do!
                            Queries
                                .InsertArticleTagsMapping
                                .Command(articleId = article.Id, tagId = tagId)
                                .Plan()
                }

            Execution.execute executionConfig insertPlan

        member this.UpdateArticle (articleId: int) (document: ParsedDocument) (tags: string list) =
            let updatePlan =
                plan {
                    do!
                        Queries
                            .DeleteArticleTagsByArticleId
                            .Command(id = articleId)
                            .Plan()

                    let! tags =
                        Plan.concurrentList [
                            for tagName in tags do
                                getTag tagName
                        ]

                    let document = { document with Tags = tags }

                    do!
                        Queries
                            .UpdateArticleById
                            .Command(
                                id = articleId,
                                title = document.Title,
                                description = document.Description,
                                source = document.Source,
                                parsed = document.Document,
                                tooltips = document.Tooltips,
                                createdOn = document.ArticleDate
                            )
                            .Plan()

                    let tagIds = tags |> List.map (fun tag -> tag.Id)

                    for tagId in batch tagIds do
                        do!
                            Queries
                                .InsertArticleTagsMapping
                                .Command(articleId = articleId, tagId = tagId)
                                .Plan()
                }

            Execution.execute executionConfig updatePlan

        member this.GetArticleById id =
            let getPlan =
                plan {
                    let! article =
                        Queries
                            .GetArticleById
                            .Command(id = id)
                            .ExactlyOne()

                    return Queries.mapArticle article
                }

            Execution.execute executionConfig getPlan

        member this.DeleteArticleById id =
            let deletePlan =
                plan {
                    do!
                        Queries
                            .DeleteArticleTagsByArticleId
                            .Command(id = id)
                            .Plan()

                    do! Queries.DeleteArticleById.Command(id = id).Plan()
                }

            Execution.execute executionConfig deletePlan

        member this.GetLatestArticle() =
            let getPlan =
                plan {
                    let! articles = Queries.GetLatestArticle.Command().Plan()

                    return
                        articles
                        |> Seq.tryHead
                        |> Option.map (Queries.mapArticle)
                }

            Execution.execute executionConfig getPlan

        member this.GetArticles() =
            let getPlan =
                plan {
                    let! articles = Queries.GetPublishedArticles.Command().Plan()

                    return articles |> Seq.map (Queries.mapArticle)
                }

            Execution.execute executionConfig getPlan

        member this.GetAllArticles() =
            let getPlan =
                plan {
                    let! articles = Queries.GetAllArticles.Command().Plan()

                    return articles |> Seq.map (Queries.mapArticle)
                }

            Execution.execute executionConfig getPlan

        member this.GetArticlesByTag(tag: string) =
            let getPlan =
                plan {
                    let! articles = Queries.GetArticlesByTag.Command(tag = tag).Plan()

                    return articles |> Seq.map (Queries.mapArticle)
                }

            Execution.execute executionConfig getPlan
