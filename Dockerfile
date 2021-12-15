FROM mcr.microsoft.com/dotnet/sdk:5.0.300-alpine3.13 AS builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet paket restore

RUN dotnet fake build -t BuildApplication

#################################

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.13

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
