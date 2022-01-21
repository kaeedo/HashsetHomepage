FROM mcr.microsoft.com/dotnet/sdk:6.0.101-alpine3.14 AS builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet paket restore

RUN dotnet fake build -t BuildApplication

#################################

FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-alpine3.14

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
