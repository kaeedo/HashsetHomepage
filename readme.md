### About

This is Blog website for personal usage.
The Upsert page is under `/articles/upsert`.

When adding or updating a post, you can add any number of Tags. A minimum of 1 is required. To do this, you can simply
click the `Tag` label on the upsert page, and a new input field will appear.

### Running locally

Dependencies

- Supabase
- .Net 8

You will also need to set several configuration options, either in the `appsettings.json` or `dotnet user-secrets` your
environment variables.
They are:

- `ConnectionString`: The postgres connection string
- `Supabase:BaseUrl`: The base URL of your supabase projects
- `Supabase:SecretApiKey`: The Secret key to access your supabase project that bypasses all client protections

Ideally, you should be able to simply run: `dotnet tool restore`, `dotnet build`, and `dotnet run`.

### Migrations

Project uses [grate](https://github.com/erikbra/grate) for DB migrations. Run the `migrations.sh` script to generate a
new migration.

Project also uses [SqlHydra](https://github.com/JordanMarr/SqlHydra) as its ORM.
Run `dotnet sqlhydra npgsql --connection-string $HASHSET_CONNECTION_STRING` to generate the F# files that map to your DB
schema
