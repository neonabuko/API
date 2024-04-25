# Song Manager API

### With this API, you can:
- Save music files
- Store music metadata
- Retrieve music metadata
- Stream music
- Delete music

### Running
This API is meant to run along with the songmanager-database and the songmanager-app. See https://github.com/neonabuko/SongManager for further instructions.

If you still wish to run it by itself, you should at least connect it to a SQL Server database, configured as follows.

In the SongManager directory, run...

```bash
docker run --network songmanager-network -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password@123" -p 5432:5432 -v ./songs:/var/opt/mssql/songs -v mssql_server:/var/opt/mssql -d --rm --name songmanager-database mcr.microsoft.com/mssql/server:latest
```

...then:

```bash
cd SongManagerAPI
dotnet run
```