using System.Text.Json;

namespace PixCollect.Configuration;

public static class ConfigurationEditor
{
    // private static string ConfigurationPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
    private const string ConfigurationPath = @"C:\Users\iperkins\Develop\PixCollect\appsettings.json";

    public static void UpdateScrapeConfiguration(ScrapeConfiguration scrapeConfiguration)
    {
        string json = File.ReadAllText(ConfigurationPath);
        Configuration configuration = JsonSerializer.Deserialize<Configuration>(json) ?? throw new NullReferenceException();
        configuration.Scrape = scrapeConfiguration;
        string updatedJson = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigurationPath, updatedJson);
    }
}