using System.Net.Sockets;
using System.Text;

class NotesClient
{
    public void Start(int port)
    {
        // Fråga användaren efter ett kommando
        Console.Write("Ange kommando: ");
        string command = Console.ReadLine() ?? "";

        // Skapa en TCP-anslutning till servern
        try
        {
            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", port);

            using NetworkStream stream = client.GetStream();

            using StreamReader reader =
                new StreamReader(stream, new UTF8Encoding(false));

            using StreamWriter writer =
                new StreamWriter(stream, new UTF8Encoding(false));

            writer.AutoFlush = true;
            writer.WriteLine(command);

            string? response = reader.ReadLine();

            // Hantera fallet där servern stänger anslutningen utan att skicka ett svar
            if (response == null)
            {
                Console.WriteLine("Servern stängde anslutningen utan svar.");
            }
            else
            {
                Console.WriteLine($"Svar: {response}");
            }
        }
        // Hantera eventuella undantag som kan uppstå vid anslutning eller kommunikation
        catch (SocketException)
        {
            Console.WriteLine(
                "Kunde inte ansluta. Kontrollera att servern är startad.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Kommunikationsfel: {ex.Message}");
        }
    }
}