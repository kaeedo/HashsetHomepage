namespace DataAccess

open Rezoom
open Rezoom.SQL
open Rezoom.SQL.Synchronous
open Rezoom.SQL.Migrations
open System.IO
open Microsoft.Data.Sqlite

type MyModel = SQLModel<".">

type MyQuery = SQL<"""
    select * FROM Articles
    WHERE Id = @id
""">

[<RequireQualifiedAccess>]
module Setup =
    let ensureDatabase () =
        let databaseFile = "./hashset.db"
        if not (File.Exists(databaseFile))
        then
            let file = FileInfo(databaseFile)
            file.Directory.Create()

            use connection = new SqliteConnection("Data Source=./hashset.db")
            connection.Open()
            //let sql = IoUtilities.getEmbeddedResource "WhereInTheWorld.Data.sqlScripts.createTables.sql"
            //connection.Execute(sql) |> ignore
            connection.Close()

    let migrate () =
        let config =
            { MigrationConfig.Default with
                LogMigrationRan = fun m -> printfn "Ran migration: %s" m.MigrationName }

        MyModel.Migrate(config)
