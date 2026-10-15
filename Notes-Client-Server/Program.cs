using System.Text.Json;
class Program
{
    static void Main(string[] args)
    {
        SettingStore store = new SettingStore();
        Settings settings = store.Load();

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
    }
}
