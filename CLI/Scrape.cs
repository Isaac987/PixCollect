using System.ComponentModel.DataAnnotations;
using Cocona;
using Cocona.Application;
using Microsoft.Extensions.Logging;
using PixCollect.Configuration;
using PixCollect.Scraping;

namespace PixCollect.CLI;

public class Scrape(ScrapeConfiguration scrapeConfiguration, ILogger<Scrape> logger)
{
    [Command(Description = "Scrape images from the web based on a given search term.")]
    public async Task Run(
        [Argument(Description = "The keyword or phrase to search for images.")] string query,
        [Argument(Description = "The maximum number of images to download.")]
        [Range(1, int.MaxValue, ErrorMessage = "The download limit must be greater than 0.")] int limit,
        [FromService] ImageScraper scraper,
        [FromService] CoconaAppContext context)
    {
        logger.LogInformation("Starting image scraping: query='{query}', limit={limit}.", query, limit);

        try
        {
            int total = await scraper.ScrapeAsync(query, limit, context.CancellationToken);
            logger.LogInformation("Image scraping completed successfully: {total} images downloaded.", total);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Image scraping failed: query='{query}', limit={limit}.", query, limit);
        }
        finally
        {
            logger.LogInformation("Image scraping task finished: query='{query}', limit={limit}.", query, limit);
        }
    }

    [Command(Description = "Enable a specific source for image scraping.")]
    public void EnableSource(
        [Argument(Description = "The name of the image source to enable (e.g., 'Google', 'Unsplash').")] string source)
    {
        logger.LogInformation("Enabling image source: '{source}'.", source);

        if (ConfigurationValidator.IsValidSource(source))
        {
            scrapeConfiguration.ScrapingSources.Add(source);
            ConfigurationEditor.UpdateScrapeConfiguration(scrapeConfiguration);
            logger.LogInformation("Image source '{source}' enabled successfully.", source);
        }
        else
        {
            logger.LogError("Invalid image source: '{source}'. Source not recognized.", source);
        }
    }

    [Command(Description = "Disable a specific source for image scraping.")]
    public void DisableSource(
        [Argument(Description = "The name of the image source to disable (e.g., 'Google', 'Unsplash').")] string source)
    {
        logger.LogInformation("Disabling image source: '{source}'.", source);

        if (scrapeConfiguration.ScrapingSources.Contains(source))
        {
            scrapeConfiguration.ScrapingSources.Remove(source);
            ConfigurationEditor.UpdateScrapeConfiguration(scrapeConfiguration);
            logger.LogInformation("Image source '{source}' disabled successfully.", source);
        }
        else
        {
            logger.LogError("Image source not found: '{source}'. Cannot disable.", source);
        }
    }

    [Command(Description = "Set the default output directory for downloaded images.")]
    public void SetOutputDirectory(
        [Argument(Description = "The directory path where images will be saved.")] string outputDirectory)
    {
        logger.LogInformation("Setting default output directory to: '{outputDirectory}'.", outputDirectory);

        scrapeConfiguration.OutputDirectory = outputDirectory;
        ConfigurationEditor.UpdateScrapeConfiguration(scrapeConfiguration);
        logger.LogInformation("Default output directory set successfully.");
    }

    [Command(Description = "Set the default format for downloaded images.")]
    public void SetFormat(
        [Argument(Description = "The desired image format (e.g., 'jpg', 'png').")] string format)
    {
        logger.LogInformation("Setting default image format to: '{format}'.", format);
        format = format.ToLower();

        if (ConfigurationValidator.IsValidFormat(format))
        {
            scrapeConfiguration.Format = format;
            ConfigurationEditor.UpdateScrapeConfiguration(scrapeConfiguration);
            logger.LogInformation("Default image format set to '{format}' successfully.", format);
        }
        else
        {
            logger.LogError("Invalid image format: '{format}'. Format not recognized.", format);
        }
    }

    [Command(Description = "Enable or disable headless mode for the scraper.")]
    public void SetHeadless(
        [Argument(Description = "Set to 'true' to enable headless mode, or 'false' to disable it.")] bool enable)
    {
        logger.LogInformation("Setting headless mode to: {enable}.", enable);

        scrapeConfiguration.Headless = enable;
        ConfigurationEditor.UpdateScrapeConfiguration(scrapeConfiguration);
    }

    [Command(Description = "Display all current configuration settings.")]
    public void ListSettings()
    {
        logger.LogInformation("Listing current configuration settings:");

        logger.LogInformation("Output Directory: {output}", scrapeConfiguration.OutputDirectory);
        logger.LogInformation("Format: {format}", scrapeConfiguration.Format);
        logger.LogInformation("Headless: {headless}", scrapeConfiguration.Headless);
        logger.LogInformation("Sources: {sources}", string.Join(", ", scrapeConfiguration.ScrapingSources));
    }
}