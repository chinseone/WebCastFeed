#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["./src/Services/WebCastFeed/", "Services/WebCastFeed/"]
COPY ["./src/Assemblies/", "Assemblies/"]

WORKDIR "/src/Services/WebCastFeed"
RUN dotnet restore "WebCastFeed.csproj"
RUN dotnet build "WebCastFeed.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebCastFeed.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# install "ping"
RUN apt-get update
RUN apt-get install -y iputils-ping

ENTRYPOINT ["dotnet", "WebCastFeed.dll"]