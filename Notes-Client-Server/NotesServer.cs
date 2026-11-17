using System.Net;
using System.Net.Sockets;
using System.Text;

class NotesServer
{
    public void Start(int port)
    {
        TcpListener server = new TcpListener(IPAddress.Loopback, port);

        try
        {
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

                if (command == "PING")
                {
                    writer.WriteLine("PONG");
                }
                else
                {
                    writer.WriteLine("ERR UNKNOWN_COMMAND");
                }
            }
        }
        finally
        {
            server.Stop();
        }
    }
}