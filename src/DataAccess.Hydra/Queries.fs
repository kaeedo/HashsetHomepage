[<RequireQualifiedAccess>]
module DataAccess.Hydra.Queries

open Npgsql
open SqlHydra.Query
open DataAccess.Hydra
open Model
open System.Threading.Tasks

let getAllTags (npgsqlDataSource: NpgsqlDataSource) =
    let contextType =
        Create(fun () ->
            let connection = npgsqlDataSource.OpenConnection()
            new QueryContext(connection, SqlKata.Compilers.PostgresCompiler()))

    selectTask HydraReader.Read contextType {
        for t in ``public``.tags do
            select t
    }

let insertArticle (npgsqlDataSource: NpgsqlDataSource) (parsed:ParsedDocument) (tags: string list)=
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

let updateArticle (npgsqlDataSource: NpgsqlDataSource) (id:int) (parsed:ParsedDocument) (tags: string list)=
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

let getArticleById (npgsqlDataSource: NpgsqlDataSource) (id:int)=
    //  Queries.GetArticleById.Command(id = id)
    Unchecked.defaultof<Task<ParsedDocument>>

let deleteArticleById (npgsqlDataSource: NpgsqlDataSource) (id:int)=
    // do!
    // Queries.DeleteArticleTagsByArticleId
    //     .Command(id = id)
    //     .Plan()
    Unchecked.defaultof<Task<unit>>

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

let getArticlesByTag (npgsqlDataSource: NpgsqlDataSource) (tag: string)=
    // let! articles = Queries.GetArticlesByTag.Command(tag = tag).Plan()
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

    type GetArticleById =
        SQL<"""
        select
            a.*,
            many tags(t.*)
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.Id = @id
    """>

    type DeleteArticleTagsByArticleId =
        SQL<"""
        delete from article_tags
        where ArticleId = @id
    """>

    type DeleteArticleById =
        SQL<"""
        delete from articles
        where Id = @id
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

    type GetArticlesByTag =
        SQL<"""
        select
            a.*,
            many tags(t.* )
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.CreatedOn <= now()
        and a.Id in (
            select a.id
            from articles a
            join article_tags at on a.Id = at.ArticleId
            join tags t on t.id = at.TagId
            where t.Name = @tag
        )
        order by a.CreatedOn desc
    """>
 *)
