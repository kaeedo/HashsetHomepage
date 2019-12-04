FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine3.9 AS builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet fake build -t Minify

#################################

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-alpine3.9

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

HEALTHCHECK --interval=5m --timeout=3s CMD curl -f http://localhost:5000/status || exit 1

ENTRYPOINT ["dotnet", "App.dll"]
