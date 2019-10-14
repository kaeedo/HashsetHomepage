namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open System

type MyModel = SQLModel<".">

type MyQuery = SQL<"""
    select * FROM Articles
    WHERE Id = @id
""">

[<RequireQualifiedAccess>]
module Db =
    let migrate () =
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        MyModel.Migrate(config)
