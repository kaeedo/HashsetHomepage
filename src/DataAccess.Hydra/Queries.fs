[<RequireQualifiedAccess>]
module DataAccess.Hydra.Queries

open Npgsql
open SqlHydra.Query
open DataAccess.Hydra

let getAllTags (npgsqlDataSource: NpgsqlDataSource) =
    let contextType =
        Create(fun () ->
            let connection = npgsqlDataSource.OpenConnection()
            new QueryContext(connection, SqlKata.Compilers.PostgresCompiler()))

    selectTask HydraReader.Read contextType {
        for t in ``public``.tags do
            select t
    }
