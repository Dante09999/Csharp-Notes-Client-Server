# Csharp Notes Client Server

Anteckningsprogram i C# med JSON-inställningar, SQLite, CRUD, transaktioner och klient–server.

## Starta

.NET 10 SDK behövs. Kör från repots huvudmapp:

```powershell
cd .\Notes-Client-Server
dotnet restore
dotnet run
```

Paketet `Microsoft.Data.Sqlite` finns i projektfilen och installeras med `dotnet restore`. Ingen separat databasserver behövs. `notes.db` skapas automatiskt med tabellen i `schema.sql`.

Starta servern och klienten i varsin terminal från projektmappen:

```powershell
dotnet run -- --server
```

```powershell
dotnet run -- --client
```

Servern kör på `127.0.0.1`, normalt port `5000`. Kör klienten igen för varje nytt kommando. Stoppa servern med Ctrl + C.

## Kommandon

Fråga och svar skickas som en rad vardera med UTF-8. Skriv kommandona med stora bokstäver.

| Fråga | Svar |
|---|---|
| `PING` | `PONG` |
| `SPARA Köp mjölk` | `OK <id>` |
| `HAMTA` | `OK <JSON-lista>`, eller `OK []` om inga anteckningar finns |
| `SPARA` utan text eller med bara mellanslag | `ERR INVALID_VALUE` |
| Okänt kommando | `ERR UNKNOWN_COMMAND` |
| Databasfel vid sparning eller hämtning | `ERR DATABASE_ERROR` |

Klienterna delar serverns databas. Om servern inte kan nås visar klienten ett felmeddelande.

## Inställningar och säkerhet

`settings.json` sparar användarnamn och port. Standardvärdena är `User` och `5000`. Saknad fil ger standardvärden. Skadad fil ger ett felmeddelande och standardvärden.

JSON är godkänt för uppgiften. C# använder `using` i stället för try-with-resources och ADO.NET i stället för JDBC.

Läs inte okända serialiserade filer: de kan innehålla manipulerade värden eller belasta programmet. Osäkra deserialiserare som BinaryFormatter kan även möjliggöra kodkörning. Programmet använder JSON till en bestämd klass och validerar värdena. Lägg inga lösenord i Git.

## Tester

```powershell
dotnet run -- --test
```

Testerna använder en tillfällig databas och skriver ut resultat för CRUD, okända id, ogiltig text, databasfel och transaktioner. Rollback testas genom att den andra ändringen misslyckas och båda posterna läses igen. Kontrollera utskrifterna; alla kontroller är inte automatiska.

Resultaten finns i [testprotokoll.md](Notes-Client-Server/testprotokoll.md).
