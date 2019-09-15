FROM fedora

RUN dnf update -y && dnf install -y libicu openssl

RUN curl -O https://download.visualstudio.microsoft.com/download/pr/d881776c-82d7-4d50-b13c-9a848da46001/d55ce79c2b3a61b303cb826b7c460d20/aspnetcore-runtime-3.0.0-preview9.19424.4-linux-x64.tar.gz

RUN mkdir -p /dotnet && tar zxf aspnetcore-runtime-3.0.0-preview9.19424.4-linux-x64.tar.gz -C /dotnet

RUN echo "export DOTNET_ROOT=/dotnet" >> /etc/bashrc

RUN echo "export PATH=$PATH:/dotnet" >> /etc/bashrc

RUN mkdir app

WORKDIR app

COPY Build .

EXPOSE 5000

ENTRYPOINT ["../dotnet/dotnet", "App.dll"]
