namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open System.IO

type internal HashsetModel = SQLModel<".">

type internal AllArticles = SQL<"""
    select * FROM articles
""">

[<RequireQualifiedAccess>]
module Setup =
    let migrate () =
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        HashsetModel.Migrate(config)
