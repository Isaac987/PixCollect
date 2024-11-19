using System.ComponentModel.DataAnnotations;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PixCollect.CLI;

public class Scrape(IConfiguration configuration, ILogger<Scrape> logger)
{   
    private IConfiguration _configuration = configuration;
    private ILogger<Scrape> _logger = logger;
    
    [Command(Description = "Scrape images from the web using a search term.")]
    public async Task ScrapeImages(
        [Argument(Description = "The keyword or phrase to search for when scraping images.")] string query,
        [Argument(Description = "The maximum number of images to download."), Range(1, int.MaxValue, ErrorMessage = "Limit must be greater than 0")] int limit,
        [Option(Description = "The directory where the downloaded images will be saved.")] string output = ".",
        [Option(Description = "The image format for the downloaded files (e.g., jpg, png).")] string format = "jpg",
        [Option(Description = "The naming pattern for the saved image files.")] string filename = ".")
    {
        logger.LogInformation("Starting Scrape Task");
    }
    
    [Command(Description = "Enable a specific image scraping source.")]
    public async Task EnableSource([Argument(Description = "The name of the image source to enable, such as 'Google' or 'Unsplash'.")] string source) 
    {
        logger.LogInformation($"Enabling source: {source}");
    }
    
    [Command(Description = "Disable a specific image scraping source.")]
    public async Task DisableSource([Argument(Description = "The name of the image source to disable, such as 'Google' or 'Unsplash'.")] string source) 
    {
        logger.LogInformation($"Disabling source: {source}");
    }
    
    [Command(Description = "Set a default configuration value for the scraper.")]
    public async Task SetDefaultValue(
        [Argument(Description = "The configuration setting to modify (e.g., 'output', 'format').")] string setting, 
        [Argument(Description = "The new value to assign to the specified configuration setting.")] string value)
    {
        _logger.LogInformation($"Setting default value: {setting} = {value}");
    }

    [Command(Description = "List all current configuration settings.")]
    public async Task ListSettings()
    {
        _logger.LogInformation("Listing all current settings...");
    }
}