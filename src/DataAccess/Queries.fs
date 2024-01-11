[<RequireQualifiedAccess>]
module DataAccess.Queries

open System
open DataAccess.``public``
open Npgsql
open SqlHydra.Query
open DataAccess
open Model
open System.Threading.Tasks

let private contextType (dataSource: NpgsqlDataSource) =
    Create(fun () ->
        let connection = dataSource.OpenConnection()
        new QueryContext(connection, SqlKata.Compilers.PostgresCompiler()))

let private mapParsedDocument (articles: (articles * article_tags * tags) seq) =
    articles
    |> Seq.groupBy (fun (a, _, _) -> a)
    |> Seq.map (fun (a, tags) ->
        let tags =
            tags
            |> Seq.toList
            |> List.map (fun (_, _, t) -> t)

        {
            ParsedDocument.Id = a.id
            Title = a.title
            Description = a.description
            ArticleDate = a.createdon
            Source = a.source
            Document = a.parsed
            Tags =
                tags
                |> List.map (fun t -> { Tag.Id = t.id; Name = t.name })
        })

let private getTags (npgsqlDataSource: NpgsqlDataSource) (tags: string seq) =
    selectTask HydraReader.Read (contextType npgsqlDataSource) {
        for tag in ``public``.tags do
            where (tag.name |=| tags)
            mapList { Tag.Id = tag.id; Name = tag.name }
    }

let private getOrCreateTags (npgsqlDataSource: NpgsqlDataSource) (allTags: string seq) =
    task {
        let! tags = getTags npgsqlDataSource allTags

        let newTags =
            allTags
            |> Seq.except (tags |> Seq.map (fun t -> t.Name))
            |> Seq.map (fun t -> { tags.id = 0; name = t })
            |> AtLeastOne.tryCreate

        match newTags with
        | Some t ->
            do!
                insertTask (contextType npgsqlDataSource) {
                    for tag in ``public``.tags do
                        entities t
                        excludeColumn tag.id
                }
                :> Task

            let! newTags =
                getTags
                    npgsqlDataSource
                    (t
                     |> AtLeastOne.getSeq
                     |> Seq.map (fun t -> t.name))

            return tags |> List.append newTags
        | None -> return tags
    }

let private deleteOrphanedTags (npgsqlDataSource: NpgsqlDataSource) =
    let usedTags =
        select {
            for at in article_tags do
                select at.tagid
        }

    deleteTask (contextType npgsqlDataSource) {
        for tag in tags do
            where (tag.id |<>| subqueryMany usedTags)
    }

let insertArticle (npgsqlDataSource: NpgsqlDataSource) (parsed: ParsedDocument) (tags: string list) =
    task {
        let! tags = getOrCreateTags npgsqlDataSource tags

        let! newArticleId =
            insertTask (contextType npgsqlDataSource) {
                for article in articles do
                    entity {
                        articles.id = 0
                        articles.createdon = DateTime.SpecifyKind(parsed.ArticleDate, DateTimeKind.Utc)
                        articles.description = parsed.Description
                        articles.parsed = parsed.Document
                        articles.title = parsed.Title
                        articles.source = parsed.Source
                    }

                    getId article.id
            }

        let document = {
            parsed with
                Id = newArticleId
                Tags = tags
        }

        let articleTags =
            tags
            |> Seq.map (fun tag -> {
                article_tags.articleid = document.Id
                tagid = tag.Id
            })
            |> AtLeastOne.tryCreate

        match articleTags with
        | Some at ->
            do!
                insertTask (contextType npgsqlDataSource) {
                    into article_tags
                    entities at
                }
                :> Task
        | None -> failwith "tags required"

        return ()
    }

let updateArticle (npgsqlDataSource: NpgsqlDataSource) (id: int) (parsed: ParsedDocument) (tags: string list) =
    task {
        let! _ =
            deleteTask (contextType npgsqlDataSource) {
                for articleTag in article_tags do
                    where (articleTag.articleid = id)
            }

        let! _ = deleteOrphanedTags npgsqlDataSource

        let! tags = getOrCreateTags npgsqlDataSource tags

        let parsed = {
            parsed with
                Tags = tags |> Seq.toList
        }

        let! _ =
            updateTask (contextType npgsqlDataSource) {
                for a in articles do
                    entity {
                        articles.id = id
                        articles.title = parsed.Title
                        articles.source = parsed.Source
                        articles.description = parsed.Description
                        articles.parsed = parsed.Document
                        articles.createdon = DateTime.SpecifyKind(parsed.ArticleDate, DateTimeKind.Utc)
                    }

                    excludeColumn a.id
                    where (a.id = id)
            }

        let articleTags =
            tags
            |> List.map (fun tag -> {
                article_tags.articleid = id
                tagid = tag.Id
            })
            |> AtLeastOne.tryCreate

        match articleTags with
        | Some at ->
            do!
                insertTask (contextType npgsqlDataSource) {
                    into article_tags
                    entities at
                }
                :> Task
        | None -> failwith "tags required"
    }

let getArticleById (npgsqlDataSource: NpgsqlDataSource) (id: int) =
    task {
        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.id = id)
            }

        let parsed = mapParsedDocument articles |> Seq.head

        return parsed
    }

let deleteArticleById (npgsqlDataSource: NpgsqlDataSource) (id: int) =
    task {
        let! _ =
            deleteTask (contextType npgsqlDataSource) {
                for articleTag in article_tags do
                    where (articleTag.articleid = id)
            }

        let! _ = deleteOrphanedTags npgsqlDataSource

        let! _ =
            deleteTask (contextType npgsqlDataSource) {
                for article in articles do
                    where (article.id = id)
            }

        return ()
    }

let getLatestArticle (npgsqlDataSource: NpgsqlDataSource) =
    task {
        let now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)

        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= now)

                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles |> Seq.tryHead

        return parsed
    }

let getPublishedArticles (npgsqlDataSource: NpgsqlDataSource) =
    task {
        let now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)

        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= now)
                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles

        return parsed
    }

let getAllArticles (npgsqlDataSource: NpgsqlDataSource) =
    task {
        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles

        return parsed
    }

let getArticlesByTag (npgsqlDataSource: NpgsqlDataSource) (filterTag: string) =
    task {
        let now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)

        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= now)
                    where (tag.name = filterTag)
                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles

        return parsed
    }
