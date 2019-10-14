module Db

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open System

type MyModel = SQLModel<"./Migrations">

type MyQuery = SQL<"""
    select * FROM Users
    WHERE Id = @id
""">

let migrate () =
    let config =
        { MigrationConfig.Default with
            LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

    MyModel.Migrate(config)

let a id =
    use connection = new ConnectionContext()
    let users = MyQuery.Command(id).Execute(connection)
    users
    |> Seq.head
