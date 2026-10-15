using System.Text.Json;
class Program
{
    static void Main(string[] args)
    {
        Settings settings = new Settings();

        Console.WriteLine($"Username: {settings.Username}");
        Console.WriteLine($"Port: {settings.Port}");

        using (FileStream stream = File.Create("settings.json"))
        {
            JsonSerializer.Serialize(stream, settings);
        }
        
        Console.WriteLine("Inställningarna har sparats.");
    }
}
