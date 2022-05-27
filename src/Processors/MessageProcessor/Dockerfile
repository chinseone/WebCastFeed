#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY ["./src/Processors/MessageProcessor/", "Processors/MessageProcessor/"]
COPY ["./src/Assemblies/", "Assemblies/"]

WORKDIR "/src/Processors/MessageProcessor"
RUN dotnet restore "MessageProcessor.csproj"
RUN dotnet build "MessageProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# install "ping"
RUN apt-get update
RUN apt-get install -y iputils-ping

ENTRYPOINT ["dotnet", "MessageProcessor.dll"]