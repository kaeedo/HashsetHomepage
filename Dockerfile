FROM frolvlad/alpine-mono:5.20-glibc as builder

RUN apk update && apk add libstdc++ && apk add libintl && apk add icu

RUN wget https://download.visualstudio.microsoft.com/download/pr/8dc8c097-42c9-4f29-ae72-90a32853fc91/a29f57092596e7e4a569ed692529dd27/dotnet-sdk-3.0.100-rc1-014190-linux-musl-x64.tar.gz

RUN mkdir -p /dotnet && tar zxf dotnet-sdk-3.0.100-rc1-014190-linux-musl-x64.tar.gz -C /dotnet

ENV DOTNET_ROOT=/dotnet
ENV PATH="/dotnet:${PATH}"
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN mkdir app

WORKDIR app

COPY . .

RUN dotnet build -c Release -o ./Build ./Homepage.sln

#################################3

FROM alpine

RUN apk update && apk add libstdc++ && apk add libintl && apk add icu

RUN wget https://download.visualstudio.microsoft.com/download/pr/60036637-9597-4155-a6dd-7c9f6646161d/a549badc03389af1bd96cd3ae781b151/aspnetcore-runtime-3.0.0-rc1.19457.4-linux-musl-x64.tar.gz

RUN mkdir -p /dotnet && tar zxf aspnetcore-runtime-3.0.0-rc1.19457.4-linux-musl-x64.tar.gz -C /dotnet

RUN echo "export DOTNET_ROOT=/dotnet" >> /etc/bashrc

RUN echo "export PATH=$PATH:/dotnet" >> /etc/bashrc

RUN mkdir app

WORKDIR app

COPY --from=builder /app/Build .

EXPOSE 5000

ENTRYPOINT ["../dotnet/dotnet", "App.dll"]
