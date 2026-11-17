using Microsoft.Data.Sqlite;

public static class RepositoryTests
{
    public static bool Run()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"notes-test-{Guid.NewGuid()}.db");
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Pooling = false
        };
        NoteRepository repository = new NoteRepository(connectionString.ToString());

        try
        {
            repository.Initialize();
            int testId = repository.Create("CRUD-test");

            Note? found = repository.GetById(testId);
            Console.WriteLine($"Hämtad: {found?.Text}");

            bool updated = repository.Update(testId, "Ändrad testtext");
            Console.WriteLine($"Ändring lyckades: {updated}");
            Console.WriteLine(
                $"Efter ändring: {repository.GetById(testId)?.Text}");

            bool deleted = repository.Delete(testId);
            Console.WriteLine($"Borttagning lyckades: {deleted}");
            Console.WriteLine(
                $"Posten är borta: {repository.GetById(testId) is null}");

            int firstId = repository.Create("Första");
            int secondId = repository.Create("Andra");

            repository.UpdateTwo(
                firstId, "Första ändrad",
                secondId, "Andra ändrad");

            Console.WriteLine(
                $"Efter commit: {repository.GetById(firstId)?.Text}");
            Console.WriteLine(
                $"Efter commit: {repository.GetById(secondId)?.Text}");

            // andra ändringen bryter mot databasens CHECK-regel
            try
            {
                repository.UpdateTwo(
                    firstId, "Ska ångras",
                    secondId, "");
            }
            catch (Microsoft.Data.Sqlite.SqliteException)
            {
                Console.WriteLine("Förväntat fel: tom text. Rollback utförd.");
            }

            Console.WriteLine(
                $"Efter rollback: {repository.GetById(firstId)?.Text}");
            Console.WriteLine(
                $"Efter rollback: {repository.GetById(secondId)?.Text}");

            // ta bort testposterna
            repository.Delete(firstId);
            repository.Delete(secondId);

            // testa ett id som inte finns
            int missingId = repository.Create("Tillfällig testpost");
            repository.Delete(missingId);

            Console.WriteLine(
                $"Okänt id, hämta: {repository.GetById(missingId) is null}");
            Console.WriteLine(
                $"Okänt id, ändra: {repository.Update(missingId, "Test")}");
            Console.WriteLine(
                $"Okänt id, ta bort: {repository.Delete(missingId)}");

            // testa tom text
            try
            {
                repository.Create("   ");
                Console.WriteLine("FEL: tom text accepterades.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Ogiltig text avvisades korrekt.");
            }

            return TestDatabaseError();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Testerna avbröts: {ex.Message}");
            return false;
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    public static bool TestDatabaseError()
    {
        // skapa inte tabellen, så att läsningen ger ett databasfel
        NoteRepository repository = new NoteRepository("Data Source=:memory:");

        try
        {
            repository.GetAll();
            Console.WriteLine("Fel: databasfelet uteblev.");
            return false;
        }
        catch (SqliteException ex) when (
            ex.SqliteErrorCode == 1 &&
            ex.Message.Contains("no such table: Notes"))
        {
            Console.WriteLine("Databasfel fångades som väntat.");
            Console.WriteLine($"Felet fångades: {ex.Message}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Oväntat fel: {ex.Message}");
            return false;
        }
    }
}
