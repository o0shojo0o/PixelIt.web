#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.14-alpine3.17-amd64 AS base
WORKDIR /
RUN apt-get update; apt-get install -y wget xz-utils fontconfig libxrender1 libxext6 libx11-6 && \
    wget https://github.com/wkhtmltopdf/wkhtmltopdf/releases/download/0.12.4/wkhtmltox-0.12.4_linux-generic-amd64.tar.xz && \
    tar Jxf wkhtmltox-0.12.4_linux-generic-amd64.tar.xz && \
	rm wkhtmltox-0.12.4_linux-generic-amd64.tar.xz && \
    cp -r wkhtmltox/bin/* /usr/bin/ ; cp -r wkhtmltox/lib/* /usr/lib/ ; cp -r wkhtmltox/include/* /usr/include/ && \
	rm -R wkhtmltox && \
   # mkdir -p /usr/share/fonts/otf ; \
   # wget https://github.com/adobe-fonts/source-han-sans/raw/release/OTF/SourceHanSansJ.zip && \
   # unzip SourceHanSansJ.zip ; mv SourceHanSansJ /usr/share/fonts/otf/ ; \
   # wget https://github.com/adobe-fonts/source-han-sans/raw/release/OTF/SourceHanSansHWJ.zip && \
   # unzip SourceHanSansHWJ.zip ; mv SourceHanSansHWJ /usr/share/fonts/otf/ ; \
   # wget https://github.com/adobe-fonts/source-han-sans/raw/release/SubsetOTF/SourceHanSansJP.zip && \
   # unzip SourceHanSansJP.zip ; mv SourceHanSansJP /usr/share/fonts/otf/ ; \
   # rm -f SourceHanSans*; fc-cache j; rm -rf /wkhtml* ;  \
    apt-get --purge remove -y wget xz-utils ; rm -rf /var/lib/apt/lists/* 
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PixelIT.web.csproj", "."]
RUN dotnet restore "./PixelIT.web.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PixelIT.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PixelIT.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelIT.web.dll"]