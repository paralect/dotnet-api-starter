#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["app/Api/Api.csproj", "app/Api/"]
COPY ["app/Common/Common.csproj", "app/Common/"]
COPY ["app/Common.DB.Mongo/Common.DB.Mongo.csproj", "app/Common.DB.Mongo/"]
COPY ["app/Common.DB.Postgres/Common.DB.Postgres.csproj", "app/Common.DB.Postgres/"]
RUN dotnet restore "app/Api/Api.csproj"
COPY . .
WORKDIR "/src/app/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]