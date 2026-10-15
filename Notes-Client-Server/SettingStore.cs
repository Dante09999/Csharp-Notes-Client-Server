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
                return settings ?? new Settings();
            }
        }
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