using System.ComponentModel.DataAnnotations;
using Cocona;
using Cocona.Application;
using PixCollect.Scraping;

namespace PixCollect.CLI;

public class Scrape(ScrapeSettings scrapeSettings, IHttpClientFactory httpClientFactory)
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
        
        // Extract the cocona context and start each scraping task
        CoconaAppContext context = contextAccessor.Current ?? throw new InvalidOperationException();
        await using PageFactory pageFactory = await PageFactory.CreateAsync();
        
        IEnumerable<Task<int>> scrapingTasks = scrapeSettings.ScrapingSources.Select(async scrapeSource =>
        {
            ImageScraper scraper = new(scrapeSource, pageFactory, httpClientFactory, scrapeSettings);
            return await scraper.ScrapeAsync(query, limit, context.CancellationToken);
        });
        
        // Count the total number of downloaded images & close the browser
        int total = (await Task.WhenAll(scrapingTasks)).Sum();
        
        Console.WriteLine($"Downloaded {total} images");
    }

    [Command(Description = "Enable a specific image scraping source.")]
    public async Task EnableSource(
        [Argument(Description = "The name of the image source to enable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
    }

    [Command(Description = "Disable a specific image scraping source.")]
    public async Task DisableSource(
        [Argument(Description = "The name of the image source to disable, such as 'Google' or 'Unsplash'.")]
        string source)
    {
    }

    [Command(Description = "Set a default configuration value for the scraper.")]
    public async Task SetDefaultValue(
        [Argument(Description = "The configuration setting to modify (e.g., 'output', 'format').")]
        string setting,
        [Argument(Description = "The new value to assign to the specified configuration setting.")]
        string value)
    {
    }

    [Command(Description = "List all current configuration settings.")]
    public async Task ListSettings()
    {
    }
}