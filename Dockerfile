FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet paket restore

RUN dotnet fake build -t Minify

#################################

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
