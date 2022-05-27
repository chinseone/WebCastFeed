#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./src/Services/XiugouChatHub/", "Services/XiugouChatHub/"]
COPY ["./src/Assemblies/", "Assemblies/"]

WORKDIR "/src/Services/XiugouChatHub"
RUN dotnet restore "XiugouChatHub.csproj"
RUN dotnet build "XiugouChatHub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XiugouChatHub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# install "ping"
RUN apt-get update
RUN apt-get install -y iputils-ping

ENTRYPOINT ["dotnet", "XiugouChatHub.dll"]