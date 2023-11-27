### About

This is Blog website for personal usage. Currently, frontend styling is written for my personal blog. Maybe in the
future I might add support for custom themes, and general blog generator features.

If you want to run this project yourself, feel free.

The Upsert page is under `/articles/upsert`.

When adding or updating a post, you can add any number of Tags. A minimum of 1 is required. To do this, you can simply
click the `Tag` label on the upsert page, and a new input field will appear.

### Running locally

Dependencies

- postgres 13
- .Net 8

You will also need to set several configuration options, either in the `appsettings.json` or your environment variables.
They are:

- `ConnectionString`: The postgres connection string
- `AuthorUsername`: The username of the author for use by Basic Authentication
- `AuthorPassword`: The password of the author for use by Basic Authentication

Ideally, you should be able to simply run: `dotnet tool restore`, `dotnet build`, and `dotnet run`.

Postgres quickstart command in
podman: `podman run -e POSTGRES_PASSWORD=password -e POSTGRES_DB=hashset -dp 5432:5432 --name postgres postgres:13-alpine`
