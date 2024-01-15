FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-alpine3.18

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY ./Release/ .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
