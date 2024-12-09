using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PixCollect.CLI;
using PixCollect.Configuration;
using PixCollect.Scraping;

namespace PixCollect;

// TODO: give better description
[HasSubCommands(typeof(Scrape), Description = "Scraping commands")]
[HasSubCommands(typeof(Upload), Description = "Uploading commands")]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);
        
        // Configure application settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        // Bind configuration section to a settings class
        ScrapeConfiguration scrapeConfiguration = new();
        builder.Configuration.GetSection("Scrape").Bind(scrapeConfiguration);
        builder.Services.AddSingleton(scrapeConfiguration);
        
        // Configure logging
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        builder.Logging.AddConsole();
        
        // Configure HTTP Client to try and prevent bot detection
        builder.Services.AddHttpClient("default", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:98.0) Gecko/20100101 Firefox/98.0");
            client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        
        // Register services
        builder.Services.AddTransient<ImageScraper>();
        builder.Services.AddTransient<SiteParserFactory>();
        
        // Build and run the application
        var app = builder.Build();
        app.AddCommands<Program>();
        app.Run();
    }
}