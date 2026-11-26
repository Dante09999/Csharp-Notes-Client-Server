using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;  

class NotesServer
{
    public void Start(int port)
    {
        TcpListener server = new TcpListener(IPAddress.Loopback, port);

        try
        {
            // förbered databasen
            NoteRepository repository = new NoteRepository();
            repository.Initialize();

            //Starta servern

            server.Start();
            Console.WriteLine($"Servern lyssnar på port {port}.");

            while (true)
            {
                using TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();

                using StreamReader reader =
                    new StreamReader(stream, new UTF8Encoding(false));

                using StreamWriter writer =
                    new StreamWriter(stream, new UTF8Encoding(false));

                writer.AutoFlush = true;

                string? command = reader.ReadLine();

                try
                {
                    //Ping-Pong
                    if (command == "PING")
                    {
                        writer.WriteLine("PONG");
                    }

                    // Hantera kommandot SPARA <text>
                    else if (command != null && command.StartsWith("SPARA "))
                    {
                        string text = command.Substring(6);
                        int id = repository.Create(text);

                        writer.WriteLine($"OK {id}");
                    }
                    // Endast SPARA utan text
                    else if (command == "SPARA")
                    {
                        writer.WriteLine("ERR INVALID_VALUE");
                    }
                    // Hantera kommandot HAMTA
                    else if (command == "HAMTA")
                    {
                        List<Note> notes = repository.GetAll();
                        string json = JsonSerializer.Serialize(notes);

                        writer.WriteLine($"OK {json}");
                    }
                    else
                    {
                        writer.WriteLine("ERR UNKNOWN_COMMAND");
                    }
                }
                catch (ArgumentException)
                {
                    writer.WriteLine("ERR INVALID_VALUE");
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"Databasfel: {ex.Message}");
                    writer.WriteLine("ERR DATABASE_ERROR");
                }
            }
        }
        finally
        {
            server.Stop();
        }
    }
}