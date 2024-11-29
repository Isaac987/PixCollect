using System.Net;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PixCollect.CLI;
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
        
        // Configure services
        builder.Services.AddHttpClient("Client", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Referer", "http://www.google.com/");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        });
        
        // Register services
        builder.Services.AddTransient<ImageScraper>();
        builder.Services.AddTransient<SiteParserFactory>();
        builder.Services.AddTransient<DownloaderFactory>();
        
        // Build and run the application
        var app = builder.Build();
        app.AddCommands<Program>();
        app.Run();
    }
}