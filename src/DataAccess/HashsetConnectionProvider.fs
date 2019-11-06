namespace Rezoom.SQL.Mapping
open System
open System.Configuration
open System.Data.Common
open System.Reflection

type HashsetConnectionProvider(connectionString: string) =
    inherit ConnectionProvider()

    override __.Open(name) =
        let provider = Npgsql.NpgsqlFactory.Instance
        let conn = provider.CreateConnection()
        conn.ConnectionString <- connectionString
        conn.Open()
        conn
