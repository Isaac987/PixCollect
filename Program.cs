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
            // .ConfigureLogging(logging =>
            // {
            //     logging.AddDebug();
            // })
            .ConfigureServices((context, services) =>
            {
                // Bind configuration to the class
                ScrapeSettings scrapeSettings = new();
                context.Configuration.GetSection("ScrapingSettings").Bind(scrapeSettings);
                services.AddSingleton(scrapeSettings);
                
                // Service Injection goes here
            }).Run<Program>(args);
    }
}