FROM mono:6 AS builder

RUN curl -O https://download.visualstudio.microsoft.com/download/pr/2c907fd7-74da-4552-bdd2-9fb0338335bc/999f25064eb476385a893f138503c9ac/dotnet-sdk-3.0.100-rc1-014190-rhel.6-x64.tar.gz

RUN mkdir -p /dotnet && tar zxf dotnet-sdk-3.0.100-rc1-014190-rhel.6-x64.tar.gz -C /dotnet

RUN echo "export DOTNET_ROOT=/dotnet" >> /etc/bashrc

RUN echo "export PATH=$PATH:/dotnet" >> /etc/bashrc

RUN mkdir app

WORKDIR app

COPY . .

CMD "dotnet build -c Release -o Build ./Homepage.sln"

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
