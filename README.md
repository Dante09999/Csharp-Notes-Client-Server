# Csharp Notes Client Server

Ett anteckningsprogram i C# för uppgift 3–6. Programmet sparar inställningar i JSON och anteckningar i SQLite. En klient kan skicka kommandon till en server som använder samma repository-klass.

## Kom igång

Du behöver .NET 10 SDK. Öppna en terminal i repots huvudmapp och gå till projektmappen:

```powershell
cd .\Notes-Client-Server
dotnet restore
dotnet build
```

Projektet använder NuGet-paketet `Microsoft.Data.Sqlite` version `10.0.13`. Paketet finns redan i projektfilen och hämtas med `dotnet restore`. Ingen separat databasserver behövs. I C# används ADO.NET med denna databasleverantör i stället för JDBC.

Kör kommandona nedan från projektmappen. Då används samma arbetsmapp för inställningsfilen och databasen.

## Vanlig körning

```powershell
dotnet run
```

Programmet läser inställningarna, visar användarnamn och port och låter dig ange ett nytt användarnamn. Enter behåller namnet. Sedan kan du spara en anteckning och se alla anteckningar.

## Server och klient

Starta servern i en terminal:

```powershell
dotnet run -- --server
```

Öppna en andra terminal i samma projektmapp och starta klienten:

```powershell
dotnet run -- --client
```

Skriv ett kommando när klienten frågar. Klienten skickar en fråga, visar svaret och avslutas. Kör klienten igen för nästa fråga. Servern fortsätter vänta på nya klienter. Stoppa servern med Ctrl + C.

Om bygget misslyckas för att programfilen används, stoppa servern och kör `dotnet build`. Starta sedan med `dotnet run --no-build -- --server` respektive `dotnet run --no-build -- --client`.

### Kommandon och svar

Servern använder `127.0.0.1` och porten i inställningarna, normalt `5000`. Kommandona skrivs med stora bokstäver. Frågor och svar skickas som UTF-8, en rad per meddelande med avslutande radbrytning. Varje anslutning hanterar en fråga och ett svar.

| Fråga | Svar | Vad som händer |
|---|---|---|
| `PING` | `PONG` | Kontrollerar kontakten med servern |
| `SPARA Köp mjölk` | Exempelvis `OK 8` | Sparar texten och returnerar dess id |
| `HAMTA` | Exempelvis `OK [{"Id":8,"Text":"Köp mjölk"}]` | Hämtar alla anteckningar i id-ordning |
| `HAMTA` när databasen är tom | `OK []` | Returnerar en tom lista |
| `SPARA` utan text eller med bara mellanslag | `ERR INVALID_VALUE` | Avvisar tom text |
| Okänt kommando, exempelvis `HEJ` | `ERR UNKNOWN_COMMAND` | Kommandot stöds inte |
| `SPARA` eller `HAMTA` vid ett SQLite-fel | `ERR DATABASE_ERROR` | Databasoperationen misslyckades |

JSON kan skriva svenska bokstäver som Unicode-koder, till exempel `\u00F6` för ö. Det är samma text. Om servern inte går att nå visar klienten ett anslutningsfel.

Alla klienter använder serverns databas. En anteckning som sparas i ett klientanrop kan hämtas i nästa.

## Databasen

Anslutningssträngen är `Data Source=notes.db`. Filen skapas i arbetsmappen. `Notes-Client-Server/schema.sql` beskriver tabellen `Notes` med kolumnerna `Id` och `Text`. Schemafilen kopieras vid bygge och körs av `Initialize()` när databasen förbereds. Befintliga anteckningar behålls.

`NoteRepository` har metoderna `Create`, `GetAll`, `GetById`, `Update` och `Delete`. Användarvärden skickas som SQL-parametrar. Ett okänt id ger `null` vid hämtning och `false` vid ändring eller borttagning. Ändring och borttagning finns i repository-klassen men har inga nätverkskommandon.

`UpdateTwo` ändrar två poster i samma transaktion. Båda ändringarna måste påverka en post var. Vid framgång görs commit, annars rollback. Testet gör den andra texten tom så att databasens CHECK-regel stoppar ändringen. Sedan läses posterna igen för att kontrollera att den första ändringen också ångrades.

SQLite-upplägget använder inga lösenord. Lägg inte eventuella framtida lösenord i källkod eller versionshantering.

## Inställningar och säkerhet

`settings.json` innehåller `Username` och `Port`. Standardvärdena är `User` och `5000`. Om filen saknas används standardvärden. Skadad JSON och läs- eller åtkomstfel ger ett meddelande och standardvärden. Tomt användarnamn eller en port utanför 1–65535 ger också standardvärden.

Inställningarna serialiseras med `System.Text.Json`. JSON har godkänts för uppgiften. C# använder `using` i stället för Javas try-with-resources för att stänga filer, databasresurser och nätverksströmmar.

Programmet ska inte läsa en okänd serialiserad fil. En fil från en okänd källa kan vara manipulerad, innehålla olämpliga värden eller vara så stor att den belastar programmet. Osäkra deserialiserare, till exempel BinaryFormatter, kan dessutom möjliggöra kodkörning när obetrodd data läses. Detta program använder JSON till en bestämd inställningsklass och kontrollerar värdena efter inläsning. Det gör inte godtyckliga filer säkra att läsa. Använd bara den egna inställningsfilen.

## Tester

Kör databastesterna separat:

```powershell
dotnet run -- --test
```

De använder en tillfällig databas som tas bort efter körningen. Testerna skriver ut resultat för CRUD, okända id, ogiltig text och transaktioner. Kontrollera utskrifterna mot förväntade resultat i testprotokollet; alla dessa kontroller är inte automatiska.

Kör bara databasfelstestet:

```powershell
dotnet run -- --test-database-error
```

Det försöker läsa en databas i minnet utan Notes-tabellen och kontrollerar att det förväntade felet fångas.

Manuella tester och resultat finns i [testprotokollet](Notes-Client-Server/testprotokoll.md). För nätverket körs klienten flera gånger mot samma server. Inställningarna kontrolleras genom omstart samt genom att tillfälligt flytta filen eller ändra dess innehåll. Ta en kopia av inställningsfilen före sådana tester.
