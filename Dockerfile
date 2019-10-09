FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine3.9 AS builder

RUN echo "http://dl-4.alpinelinux.org/alpine/edge/main" >> /etc/apk/repositories && \
    echo "http://dl-4.alpinelinux.org/alpine/edge/community" >> /etc/apk/repositories && \
    echo "http://dl-4.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories && \
    apk add --no-cache \
    bash \
    curl \
    make \
    mono \
    ca-certificates \
    && rm -rf /var/cache/apk/*

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN cert-sync /etc/ssl/certs/ca-certificates.crt

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet tool restore

RUN dotnet fake build

#################################

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-alpine3.9

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY --from=builder /app/build .

EXPOSE 5000

ENTRYPOINT ["dotnet", "App.dll"]
