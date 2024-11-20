using System.ComponentModel.DataAnnotations;
using Cocona;
using Cocona.Application;
using Microsoft.Extensions.Logging;
using PixCollect.Scraping;

namespace PixCollect.CLI;

public class Scrape(ScrapeSettings scrapeSettings, ILogger<Scrape> logger, ILoggerFactory loggerFactory)
{
    [Command(Description = "Scrape images from the web using a search term.")]
    public async Task Run(
        [Argument(Description = "The keyword or phrase to search for when scraping images.")]
        string query,
        [Argument(Description = "The maximum number of images to download.")]
        [Range(1, int.MaxValue, ErrorMessage = "Limit must be greater than 0")]
        int limit,
        [FromService] ICoconaAppContextAccessor contextAccessor)
    {
        logger.LogInformation("Starting Image Scrape: query={query}, limit={limit}", query, limit);

        CoconaAppContext context = contextAccessor.Current ?? throw new InvalidOperationException();
        IEnumerable<Task<int>> scrapingTasks = scrapeSettings.ScrapingSources.Select(async scrapeSource =>
        {
            ImageScraper scraper = new(scrapeSource, scrapeSettings, loggerFactory.CreateLogger<ImageScraper>());
            return await scraper.ScrapeAsync(query, limit, context.CancellationToken);
        });
        
        int total = (await Task.WhenAll(scrapingTasks)).Sum();
        
        logger.LogInformation($"Ending Image Scrape: total={total}");
    }

    [Command(Description = "Enable a specific image scraping source.")]
    public async Task EnableSource(
        [Argument(Description = "The name of the image source to enable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
        logger.LogInformation($"Enabling source: {source}");
    }

    [Command(Description = "Disable a specific image scraping source.")]
    public async Task DisableSource(
        [Argument(Description = "The name of the image source to disable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
        logger.LogInformation($"Disabling source: {source}");
    }

    [Command(Description = "Set a default configuration value for the scraper.")]
    public async Task SetDefaultValue(
        [Argument(Description = "The configuration setting to modify (e.g., 'output', 'format').")]
        string setting,
        [Argument(Description = "The new value to assign to the specified configuration setting.")]
        string value)
    {
        logger.LogInformation($"Setting default value: {setting} = {value}");
    }

    [Command(Description = "List all current configuration settings.")]
    public async Task ListSettings()
    {
        logger.LogInformation("Listing all current settings...");
    }
}