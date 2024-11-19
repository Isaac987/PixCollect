using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PixCollect.CLI;

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
            .ConfigureLogging(logging =>
            {
                logging.AddDebug();
            })
            .ConfigureServices(services =>
            {
                // Service Injection goes here
            }).Run<Program>(args);
    }
}