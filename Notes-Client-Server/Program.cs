class Program
{
    static void Main(string[] args)
    {
        // kör tester om man väljer testläge
        if (args.Contains("--test-database-error"))
        {
            Environment.ExitCode = RepositoryTests.TestDatabaseError() ? 0 : 1;
            return;
        }

        if (args.Contains("--test"))
        {
            Environment.ExitCode = RepositoryTests.Run() ? 0 : 1;
            return;
        }

        // ladda inställningar
        SettingStore store = new SettingStore();
        Settings settings = store.Load();

        // starta servern om man väljer serverläge
        if (args.Contains("--server"))
        {
            try
            {
                NotesServer server = new NotesServer();
                server.Start(settings.Port);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine($"Servern kunde inte köras: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Kommunikationsfel: {ex.Message}");
            }

            return;
        }

        // starta klienten
        if (args.Contains("--client"))
        {
            NotesClient client = new NotesClient();
            client.Start(settings.Port);
            return;
        }

        // förbered databasen
        NoteRepository repository = new NoteRepository();
        try
        {
            repository.Initialize();
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex)
        {
            Console.WriteLine($"Databasen kunde inte startas: {ex.Message}");
            Environment.ExitCode = 1;
            return;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Schemafilen kunde inte läsas: {ex.Message}");
            Environment.ExitCode = 1;
            return;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Åtkomst till schemafilen saknas: {ex.Message}");
            Environment.ExitCode = 1;
            return;
        }

        Console.WriteLine($"Användare: {settings.Username}");
        Console.WriteLine($"Port: {settings.Port}");

        Console.Write("Ange ett nytt användarnamn, eller tryck Enter: ");
        string? newUsername = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(newUsername))
        {
            settings.Username = newUsername;

            try
            {
                store.Save(settings);
                Console.WriteLine("Inställningarna har sparats.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Åtkomst till inställningsfilen saknas. Inställningarna kunde inte sparas.");
            }
            catch (IOException)
            {
                Console.WriteLine("Inställningsfilen kunde inte skrivas. Inställningarna kunde inte sparas.");
            }
        }

        // skapa en anteckning
        Console.Write("Skriv en anteckning: ");
        string text = Console.ReadLine() ?? "";

        try
        {
            int id = repository.Create(text);
            Console.WriteLine($"Anteckningen sparades med id {id}.");

            // visa alla anteckningar
            Console.WriteLine("\nAlla anteckningar:");

            List<Note> notes = repository.GetAll();

            foreach (Note note in notes)
            {
                Console.WriteLine($"{note.Id}: {note.Text}");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex)
        {
            Console.WriteLine($"Databasfel: {ex.Message}");
        }
    }
}
