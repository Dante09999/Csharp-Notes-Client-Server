using System;
class Program
{
    static void Main(string[] args)
    {
        Settings settings = new Settings();

        Console.WriteLine($"Username: {settings.Username}");
        Console.WriteLine($"Port: {settings.Port}");
    }
}
