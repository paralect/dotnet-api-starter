#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["app/SignalR/SignalR.csproj", "app/SignalR/"]
COPY ["app/Common/Common.csproj", "app/Common/"]
RUN dotnet restore "app/SignalR/SignalR.csproj"
COPY . .
WORKDIR "/src/app/SignalR"
RUN dotnet build "SignalR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y netcat-openbsd

COPY ["app/SignalR/bash/", "bash"]
ENTRYPOINT ["/bin/bash", "bash/setup.sh", "api", "3001"]