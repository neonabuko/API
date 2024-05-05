# ScoreHub API

### With this API, you can create, read, update and delete songs and music scores.

### Running
This API is meant to run along with ScoreHub-App. See https://github.com/neonabuko/SongManager for further instructions.

If you still wish to run it by itself, you should at least connect it to a SQL Server database, configured as follows.

In the ScoreHub-API directory, run:

```bash
docker run --network songmanager-network -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password@123" -p 5432:5432 -v ./songs:/var/opt/mssql/songs -v mssql_server:/var/opt/mssql -d --rm --name songmanager-database_c mcr.microsoft.com/mssql/server:latest
```

then:

```bash
dotnet run
```