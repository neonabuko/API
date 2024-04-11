# BACKEND
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "./SongManager.csproj" --disable-parallel
RUN dotnet publish "./SongManager.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app .

USER app
EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "SongManager.dll"]