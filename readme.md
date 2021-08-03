### About
This is Blog website for personal usage. Currently, frontend styling is written for my personal blog. Maybe in the future I might add support for custom themes, and general blog generator features.

If you want to run this project yourself, feel free, just keep the following in mind.

You need to register an OAuth application in Github to be able to authenticate to allow upserting blog posts.
The actual upsert page is under `/articles/upsert/0`. Use 0 to add a new post, or any ID to update a post.

When adding or updating a post, you can add any number of Tags. To do this, you can simply click the `Tag` label on the upsert page, and a new input field will appear.

This site supports a commenting system powered by [Commento](https://www.commento.io/), and if you want to run your own site, make sure to replace the URL of where commento is hosted in `src/App/Views/Article.fs`.

### Running locally
Dependencies
* postgres
* .Net 5

You will need to generate a self signed certificate for local development using `dotnet dev-certs https --trust`. More info here: https://www.hanselman.com/blog/DevelopingLocallyWithASPNETCoreUnderHTTPSSSLAndSelfSignedCerts.aspx

You will also need to set several configuration options, either in the `appsettings.json`, your environment variables, or using `dotnet user-secrets` as [detailed here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.0&tabs=windows).
They are:
* `ConnectionString`: The postgres connection string
* `GithubClientId`: The client ID from your github OAuth application
* `GithubClientSecret`: The client secret from your github OAuth application
* `GithubWriteUsername`: The github username that is authorized to write blog posts (I know, shouldn't use OAuth for authorization. This also only allows a single account to write posts...)

Ideally, you should be able to simply run: `dotnet tool restore`, `dotnet build`, and `dotnet run`. It installs `paket` and `fake-cli` as local tools and they should be good to go out of the box.

Postgres quickstart command in docker: `docker run -e POSTGRES_PASSWORD=password -dp 5432:5432 --name postgres postgres:13-alpine`

### Building a docker container
This is where the `fake` build script comes into play. `fake build` will read the latest version from `release-notes.md` and build a docker container with that version as the tag. The build script requires your docker username as the environment variable `USERNAME`, with which it will tag the docker image
