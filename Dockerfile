FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./SongManager.csproj .

RUN dotnet restore "./SongManager.csproj" --disable-parallel

COPY . .

RUN dotnet publish "./SongManager.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app .

USER root
EXPOSE 5000
EXPOSE 7196

ENTRYPOINT ["dotnet", "SongManager.dll"]