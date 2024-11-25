using System.Net;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PixCollect.CLI;
using PixCollect.Scraping;

namespace PixCollect;

// TODO: give better description
[HasSubCommands(typeof(Scrape), Description = "Scraping commands")]
[HasSubCommands(typeof(Upload), Description = "Uploading commands")]
public class Program(IConfiguration configuration, ILogger<Program> logger)
{
    private IConfiguration _configuration = configuration;
    private ILogger<Program> _logger = logger;

    public static void Main(string[] args)
    {
        CoconaApp.CreateHostBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            })
            .ConfigureServices((context, services) =>
            {
                // Bind configuration to the class
                ScrapeSettings scrapeSettings = new();
                context.Configuration.GetSection("ScrapingSettings").Bind(scrapeSettings);
                services.AddSingleton(scrapeSettings);
                   
                // Add httpclient with standard headers
                services.AddHttpClient("Client", client =>
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
                
                // Service Injection goes here
                
            }).Run<Program>(args);
    }
}