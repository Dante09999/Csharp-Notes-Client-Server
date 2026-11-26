using System.Text.Json;
class SettingStore
{
    private const string SettingsFilePath = "settings.json";

    public void Save(Settings settings)
    {
        using (FileStream stream = File.Create(SettingsFilePath))
        {
            JsonSerializer.Serialize(stream, settings);
        }
    }

    public Settings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                return new Settings();
            }

            using (FileStream stream = File.OpenRead(SettingsFilePath))
            {
                Settings? settings = JsonSerializer.Deserialize<Settings>(stream);

                if (settings == null)
                {
                    Console.WriteLine("Inställningsfilen är tom. Standardvärden används.");
                    return new Settings();
                }

                if (string.IsNullOrWhiteSpace(settings.Username))
                {
                    Console.WriteLine("Användarnamnet i inställningsfilen är tomt. Standardvärden används.");
                    return new Settings();
                }

                if (settings.Port <= 0 || settings.Port > 65535)
                {
                    Console.WriteLine("Porten i inställningsfilen är ogiltig. Standardvärden används.");
                    return new Settings();
                }

                return settings;

            }
        }
        // Hantera eventuella undantag som kan uppstå vid läsning av filen
        catch (JsonException)
        {
            Console.WriteLine("Inställningsfilen är skadad. Standardvärden används.");
            return new Settings();
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Åtkomst till inställningsfilen saknas. Standardvärden används.");
            return new Settings();
        }
        catch (IOException)
        {
            Console.WriteLine("Inställningsfilen kunde inte läsas. Standardvärden används.");
            return new Settings();
        }
    }
}
