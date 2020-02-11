namespace DataAccess

open System
open System.Collections.Generic
open Rezoom.SQL
open Model

[<RequireQualifiedAccess>]
module Queries =
    type HashsetModel = SQLModel<".">

    type InsertTag = SQL<"""
        insert into tags
        row
            Name = @name;
        select lastval() as Id;
    """>

    type GetTag = SQL<"""
        select * from tags
        where Name = @name
    """>

    type InsertArticle = SQL<"""
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

    type InsertArticleTagsMapping = SQL<"""
        insert into article_tags
        row
            ArticleId = @articleId,
            TagId = @tagId
    """>

    type UpdateArticleById = SQL<"""
        update articles set
            Title = @title,
            Source = @source,
            Description = @description,
            Parsed = @parsed,
            Tooltips = @tooltips,
            CreatedOn = @createdOn
        where Id = @id
    """>

    type GetLatestArticle = SQL<"""
        select
            a.*,
            many tags(t.*)
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.CreatedOn <= now()
        order by a.CreatedOn desc
        limit (select count(*) as count
                from articles a
                join article_tags at on a.Id = at.ArticleId
                join tags t on t.id = at.TagId)
    """>

    type GetArticleById = SQL<"""
        select
            a.*,
            many tags(t.*)
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.Id = @id
    """>

    type DeleteArticleTagsByArticleId = SQL<"""
        delete from article_tags
        where ArticleId = @id
    """>

    type DeleteArticleById = SQL<"""
        delete from articles
        where Id = @id
    """>

    type GetArticles = SQL<"""
        select
            a.*,
            many tags(t.*)
        from articles a
        join article_tags at on a.Id = at.ArticleId
        join tags t on t.id = at.TagId
        where a.CreatedOn <= now()
        order by a.CreatedOn desc
    """>

    type GetArticlesByTag = SQL<"""
        select
            a.*,
            many tags(t.*)
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

    let inline mapArticle row =
        { ParsedDocument.Id = (^a: (member get_Id: unit -> int)(row))
          Title = (^a: (member get_Title: unit -> string)(row))
          ArticleDate = (^a: (member get_CreatedOn: unit -> DateTime)(row))
          Description = (^a: (member get_Description: unit -> string)(row))
          Source = (^a: (member get_Source: unit -> string)(row))
          Document = (^a: (member get_Parsed: unit -> string)(row))
          Tooltips = (^a: (member get_Tooltips: unit -> string)(row))
          Tags =
                (^a: (member get_tags: unit -> IReadOnlyList<_>)(row))
                |> Seq.map (fun tagRow ->
                    { Tag.Id = (^b: (member get_Id: unit -> int)(tagRow))
                      Name = (^b: (member get_Name: unit -> string)(tagRow)) }
                )
                |> Seq.toList
        }
