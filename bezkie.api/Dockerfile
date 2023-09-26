#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 3000

ENV ASPNETCORE_URLS=http://+:3000
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["bezkie.api/bezkie.api.csproj", "bezkie.api/"]
COPY ["bezkie.application/bezkie.application.csproj", "bezkie.application/"]
COPY ["bezkie.core/bezkie.core.csproj", "bezkie.core/"]
COPY ["bezkie.infrastructure/bezkie.infrastructure.csproj", "bezkie.infrastructure/"]
RUN dotnet restore "bezkie.api/bezkie.api.csproj"
COPY . .
WORKDIR "/src/bezkie.api"
RUN dotnet build "bezkie.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bezkie.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bezkie.api.dll"]