### About

This is Blog website for personal usage. Currently, frontend styling is written for my personal blog. Maybe in the future I might add support for custom themes, and general blog generator features.

If you want to run this project yourself, feel free.

The Upsert page is under `/articles/upsert`.

When adding or updating a post, you can add any number of Tags. A minimum of 1 is required. To do this, you can simply click the `Tag` label on the upsert page, and a new input field will appear.

### Running locally

Dependencies

- postgres 13
- .Net 6

You will need to generate a self signed certificate for local development using `dotnet dev-certs https --trust`. More info here: https://www.hanselman.com/blog/DevelopingLocallyWithASPNETCoreUnderHTTPSSSLAndSelfSignedCerts.aspx

Hashset uses basic authentication for the author to "log in". SSL is a must in production (should be regardless).

You will also need to set several configuration options, either in the `appsettings.json`, your environment variables, or using `dotnet user-secrets` as [detailed here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.0&tabs=windows).
They are:

- `ConnectionString`: The postgres connection string
- `AuthorUsername`: The username of the author for use by Basic Authentication
- `AuthorPassword`: The password of the author for use by Basic Authentication

Ideally, you should be able to simply run: `dotnet tool restore`, `dotnet build`, and `dotnet run`. It installs `paket` and `fake-cli` as local tools and they should be good to go out of the box.

Postgres quickstart command in docker: `docker run -e POSTGRES_PASSWORD=password -e POSTGRES_DB=hashset -dp 5432:5432 --name postgres postgres:13-alpine`

### Building a docker container

This is where the `fake` build script comes into play. `fake build` will read the latest version from `release-notes.md` and build a docker container with that version as the tag. The build script requires your docker username as the environment variable `USERNAME`, with which it will tag the docker image
