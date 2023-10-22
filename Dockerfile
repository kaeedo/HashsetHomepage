FROM mcr.microsoft.com/dotnet/sdk:7.0.402-alpine3.18 AS builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet build -configuration Release

#################################

FROM mcr.microsoft.com/dotnet/aspnet:7.0.12-alpine3.18

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
