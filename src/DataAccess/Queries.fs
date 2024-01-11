[<RequireQualifiedAccess>]
module DataAccess.Queries

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

let getAllTags (npgsqlDataSource: NpgsqlDataSource) =
    selectTask HydraReader.Read (contextType npgsqlDataSource) {
        for t in tags do
            select t
    }

let insertArticle (npgsqlDataSource: NpgsqlDataSource) (parsed: ParsedDocument) (tags: string list) =
    //     plan {
    //     let! tags =
    //         Plan.concurrentList [
    //             for tagName in tags do
    //                 getTag tagName
    //         ]

    //     let document = {
    //         document with
    //             Tags = [ { Tag.Id = 8; Name = "F#" } ]
    //     }

    //     let! article =
    //         Queries.InsertArticle
    //             .Command(
    //                 title = document.Title,
    //                 source = document.Source,
    //                 description = document.Description,
    //                 parsed = document.Document,
    //                 tooltips = document.Tooltips,
    //                 createdOn = DateTime.SpecifyKind(document.ArticleDate, DateTimeKind.Utc)
    //             )
    //             .Plan()

    //     let tagIds = tags |> List.map (fun tag -> tag.Id)

    //     for tagId in batch tagIds do
    //         do!
    //             Queries.InsertArticleTagsMapping
    //                 .Command(articleId = article.Id, tagId = tagId)
    //                 .Plan()
    // }
    Unchecked.defaultof<Task<unit>>

let updateArticle (npgsqlDataSource: NpgsqlDataSource) (id: int) (parsed: ParsedDocument) (tags: string list) =
    //  plan {
    //     do!
    //         Queries.DeleteArticleTagsByArticleId
    //             .Command(id = articleId)
    //             .Plan()

    //     let! tags =
    //         Plan.concurrentList [
    //             for tagName in tags do
    //                 getTag tagName
    //         ]

    //     let document = { document with Tags = tags }

    //     do!
    //         Queries.UpdateArticleById
    //             .Command(
    //                 id = articleId,
    //                 title = document.Title,
    //                 description = document.Description,
    //                 source = document.Source,
    //                 parsed = document.Document,
    //                 tooltips = document.Tooltips,
    //                 createdOn = DateTime.SpecifyKind(document.ArticleDate, DateTimeKind.Utc)
    //             )
    //             .Plan()

    //     let tagIds = tags |> List.map (fun tag -> tag.Id)

    //     for tagId in batch tagIds do
    //         do!
    //             Queries.InsertArticleTagsMapping
    //                 .Command(articleId = articleId, tagId = tagId)
    //                 .Plan()
    // }
    Unchecked.defaultof<Task<unit>>

let getArticleById (npgsqlDataSource: NpgsqlDataSource) (id: int) =
    task {
        let! article =
            selectTask HydraReader.Read (contextType npgsqlDataSource) {
                for article in articles do
                    join at in article_tags on (article.id = at.articleid)
                    join tag in tags on (at.tagid = tag.id)
                    where (article.id = id)
            }

        let parsed =
            article
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
            |> Seq.head

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
    // let! articles = Queries.GetLatestArticle.Command().Plan()

    // return
    // articles
    // |> Seq.tryHead
    // |> Option.map (Queries.mapArticle)
    Unchecked.defaultof<Task<ParsedDocument option>>

let getArticles (npgsqlDataSource: NpgsqlDataSource) =
    // let! articles = Queries.GetPublishedArticles.Command().Plan()
    Unchecked.defaultof<Task<ParsedDocument list>>

let getAllArticles (npgsqlDataSource: NpgsqlDataSource) =
    // let! articles = Queries.GetAllArticles.Command().Plan()
    Unchecked.defaultof<Task<ParsedDocument list>>

let getArticlesByTag (npgsqlDataSource: NpgsqlDataSource) (tag: string) =
    // select
    //         a.*,
    //         many tags(t.* )
    //     from articles a
    //     join article_tags at on a.Id = at.ArticleId
    //     join tags t on t.id = at.TagId
    //     where a.CreatedOn <= now()
    //     and a.Id in (
    //         select a.id
    //         from articles a
    //         join article_tags at on a.Id = at.ArticleId
    //         join tags t on t.id = at.TagId
    //         where t.Name = @tag
    //     )
    //     order by a.CreatedOn desc
    Unchecked.defaultof<Task<ParsedDocument list>>


(*
    type InsertTag =
        SQL<"""
        insert into tags
        row
            Name = @name;
        select lastval() as Id;
    """>

    type GetTag =
        SQL<"""
        select * from tags
        where Name = @name
    """>

    type InsertArticle =
        SQL<"""
        insert into articles
        row
            Title = @title,
            Source = @source,
            Description = @description,
            Parsed = @parsed,
            Tooltips = @tooltips,
            CreatedOn = @createdOn;
        select lastval() as Id;
    """>

    type InsertArticleTagsMapping =
        SQL<"""
        insert into article_tags
        row
            ArticleId = @articleId,
            TagId = @tagId
    """>

    type UpdateArticleById =
        SQL<"""
        update articles set
            Title = @title,
            Source = @source,
            Description = @description,
            Parsed = @parsed,
            Tooltips = @tooltips,
            CreatedOn = @createdOn
        where Id = @id
    """>

    type GetLatestArticle =
        SQL<"""
        select
            a.*,
            many tags(t.* )
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.CreatedOn <= now()
        order by a.CreatedOn desc
        limit (select count(* ) as count
                from articles a
                join article_tags at on a.Id = at.ArticleId
                join tags t on t.id = at.TagId)
    """>


    type DeleteArticleTagsByArticleId =
        SQL<"""
        delete from article_tags
        where ArticleId = @id
    """>



    type GetPublishedArticles =
        SQL<"""
        select
            a.*,
            many tags(t. * )
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.CreatedOn <= now()
        order by a.CreatedOn desc
    """>

    type GetAllArticles =
        SQL<"""
        select
            a.*,
            many tags(t.* )
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        order by a.CreatedOn desc
    """>
 *)
