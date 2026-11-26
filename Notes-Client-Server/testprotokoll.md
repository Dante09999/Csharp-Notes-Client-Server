# Testprotokoll

| Test | Förväntat resultat | Faktiskt resultat | Status |
|---|---|---|---|
| Skapa och hämta | Texten CRUD-test hämtas | CRUD-test hämtades | Godkänt |
| Ändra | Ny text sparas | Ändrad testtext hämtades | Godkänt |
| Ta bort | Posten finns inte kvar | GetById returnerade null | Godkänt |
| Transaktion med commit | Båda posterna ändras | Båda fick ändrad text | Godkänt |
| Transaktion med rollback | Första ändringen ångras när andra misslyckas | Första ändrad och Andra ändrad fanns kvar | Godkänt |
| Okänt id, hämta | Returnerar null | Returnerade null | Godkänt |
| Okänt id, ändra | Returnerar false | Returnerade false | Godkänt |
| Okänt id, ta bort | Returnerar false | Returnerade false | Godkänt |
| Ogiltigt värde | Text med bara mellanslag avvisas | ArgumentException fångades | Godkänt |
| Databasfel | Saknad Notes-tabell ger ett fel som fångas | SQLite Error 1: no such table: Notes fångades; testet avslutades med kod 0 | Godkänt |

| PING | PONG | PONG | Godkänt |
| SPARA och HAMTA | Sparad anteckning hämtas i nästa klientanrop | Anteckning med id 8 hämtades | Godkänt |
| Svenska tecken | åäö bevaras | Svenska tecken återgavs som Unicode-koder i JSON | Godkänt |
| Tom SPARA | ERR INVALID_VALUE | ERR INVALID_VALUE | Godkänt |
| Okänt kommando | ERR UNKNOWN_COMMAND | ERR UNKNOWN_COMMAND | Godkänt |
| Onåbar server | Begripligt anslutningsfel utan krasch | Kunde inte ansluta. Kontrollera att servern är startad. | Godkänt |

De fyra testerna för okänt id och ogiltigt värde verifierades 2026-11-17 genom programkörning med en separat tillfällig databas.

Databasfelstestet verifierades 2026-11-17 med `dotnet run --project .\Notes-Client-Server\Notes-Client-Server.csproj -- --test-database-error` från repots huvudmapp. Testet använder en separat databas i minnet utan schema och påverkar inte notes.db.

Kör databastesterna separat från projektmappen med `dotnet run -- --test`. Testerna använder en tillfällig databas som tas bort efteråt. Vanlig körning med `dotnet run` skapar inga testposter.

Nätverkstesterna dokumenterades 2026-11-26. Kommandosvaren bygger på de manuella körningar som visats i chatten. Onåbar server verifierades separat med den byggda klienten mot en lokal port utan lyssnande server, med tillfälliga inställningar. Klienten avslutades normalt.
