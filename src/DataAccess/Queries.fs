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
            Tooltips = ""
            Tags =
                tags
                |> List.map (fun t -> { Tag.Id = t.id; Name = t.name })
        })

let getAllTags (npgsqlDataSource: NpgsqlDataSource) =
    selectTask HydraReader.Read (contextType npgsqlDataSource) {
        for t in tags do
            select t
    }

let insertArticle (npgsqlDataSource: NpgsqlDataSource) (parsed: ParsedDocument) (tags: string list) =
    task {
        let! tags =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for tag in ``public``.tags do
                    where (tag.name |=| tags)
                    mapList { Tag.Id = tag.id; Name = tag.name }
            }

        let document = { parsed with Tags = tags }

        let! newArticleId =
            insertTask (contextType npgsqlDataSource) {
                for article in articles do
                    entity {
                        articles.id = 0
                        articles.createdon = document.ArticleDate
                        articles.description = document.Description
                        articles.parsed = document.Document
                        articles.title = document.Title
                        articles.source = document.Source
                    }

                    getId article.id
            }

        let articleTags =
            tags
            |> List.map (fun tag -> {
                article_tags.articleid = parsed.Id
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

        let! tags =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for tag in ``public``.tags do
                    where (tag.name |=| tags)
                    mapList { Tag.Id = tag.id; Name = tag.name }
            }

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
                        articles.createdon = parsed.ArticleDate
                    }

                    excludeColumn a.id
                    where (a.id = id)
            }

        let articleTags =
            tags
            |> List.map (fun tag -> {
                article_tags.articleid = parsed.Id
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
                for article in articles do
                    where (article.id = id)
            }

        return ()
    }

let getLatestArticle (npgsqlDataSource: NpgsqlDataSource) =
    task {
        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= DateTime.Now)
                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles |> Seq.tryHead

        return parsed
    }

let getArticles (npgsqlDataSource: NpgsqlDataSource) =
    task {
        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= DateTime.Now)
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
        let! articles =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.createdon <= DateTime.Now)
                    where (tag.name = filterTag)
                    orderByDescending article.createdon
            }

        let parsed = mapParsedDocument articles

        return parsed
    }
