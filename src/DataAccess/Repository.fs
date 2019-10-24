namespace DataAccess

open Rezoom.SQL
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open Model

type internal HashsetModel = SQLModel<".">

type internal InsertTag = SQL<"""
    insert into tags
    row
        Name = @name;
    select lastval() as tagId;
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
    select lastval() as tagId;
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

    let private insertArticleTagsMapping (articleId: int) (tagIds: int list) (context: ConnectionContext) =
        tagIds
        |> List.iter (fun tagId ->
            InsertArticleTagsMapping.Command(articleId = articleId, tagId = tagId).Execute(context)
        )
        ()

    let insertTag (name: string) (context: ConnectionContext) =
        let tagId = InsertTag.Command(name = name).ExecuteScalar(context)

        { Tag.Id = tagId; Name = name }

    let getTags (names: string list) =
        use context = new ConnectionContext()

        names
        |> List.map (fun name ->
            let persistedTag =
                GetTag.Command(name = name).ExecuteTryExactlyOne(context)

            match persistedTag with
            | None -> insertTag name context
            | Some tag -> { Tag.Id = tag.Id; Name = tag.Name }
        )

    let insertArticle (document: ParsedDocument) (tags: string list) =
        let (tags: Tag list) = getTags tags
        let document = { document with Tags = Some tags }

        use context = new ConnectionContext()

        let articleId =
            InsertArticle.Command(
                title = document.Title,
                source = document.Source,
                parsed = document.Document,
                tooltips = document.Tooltips,
                createdOn = document.ArticleDate
                )
                .ExecuteScalar(context)

        let tagIds = tags |> List.map (fun tag -> tag.Id)
        insertArticleTagsMapping articleId tagIds context
        ()
