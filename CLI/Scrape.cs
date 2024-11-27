using System.ComponentModel.DataAnnotations;
using Cocona;
using Cocona.Application;
using Microsoft.Extensions.Logging;
using PixCollect.Scraping;

namespace PixCollect.CLI;

public class Scrape
{
    private readonly ILogger<Scrape> _logger;
    
    public Scrape(ILogger<Scrape> logger)
    {
        _logger = logger;
    }
    
    [Command(Description = "Scrape images from the web using a search term.")]
    public async Task Run(
        [Argument(Description = "The keyword or phrase to search for when scraping images.")] string query,
        [Argument(Description = "The maximum number of images to download.")]
        [Range(1, int.MaxValue, ErrorMessage = "Limit must be greater than 0")] int limit,
        [FromService] ImageScraper scraper,
        [FromService] CoconaAppContext context)
    {
        _logger.LogInformation("Starting scrape run: query='{query}', limit={limit}.", query, limit);

        try
        {
            int total = await scraper.ScrapeAsync(query, limit, context.CancellationToken);
            _logger.LogInformation("Completed scrape run: total={total}.", total);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Scrape Run failed: query='{query}', limit={limit}.", query, limit);
        }
        finally
        {
            _logger.LogInformation("Scrape run finished: query='{query}', limit={limit}.", query, limit);
        }
    }

    [Command(Description = "Enable a specific image scraping source.")]
    public void EnableSource(
        [Argument(Description = "The name of the image source to enable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
        _logger.LogInformation("Starting Task: Scrape EnableSource");

    }

    [Command(Description = "Disable a specific image scraping source.")]
    public void DisableSource(
        [Argument(Description = "The name of the image source to disable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
        _logger.LogInformation("Starting Task: Scrape DisableSource");
    }

    [Command(Description = "Set a default configuration value for the scraper.")]
    public void SetDefaultValue(
        [Argument(Description = "The configuration setting to modify (e.g., 'output', 'format').")]
        string setting,
        [Argument(Description = "The new value to assign to the specified configuration setting.")]
        string value)
    {
        _logger.LogInformation("Starting Task: Scrape SetDefaultValue");
    }

    [Command(Description = "List all current configuration settings.")]
    public void ListSettings()
    {
        _logger.LogInformation("Starting Task: Scrape ListSettings");

    }
}