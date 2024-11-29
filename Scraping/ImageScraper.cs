using Microsoft.Extensions.Logging;

namespace PixCollect.Scraping;

public sealed class ImageScraper(
    ScrapeConfiguration scrapeConfiguration,
    SiteParserFactory siteParserFactory,
    DownloaderFactory downloaderFactory,
    ILogger<ImageScraper> logger)
{
    public async Task<int> ScrapeAsync(string query, int limit, CancellationToken cancellationToken)
    {
        // Create the scraping session directory
        string outputDirectory = ScrapeSession.CreateSessionDirectory(query, scrapeConfiguration.OutputDirectory);
        logger.LogInformation("Created scrape session: directory='{outputDirectory}'", outputDirectory);

        IEnumerable<Task<int>> scrapingTasks = scrapeConfiguration.ScrapingSources.Select(async scrapeSource =>
        {
            logger.LogInformation("Starting image scraper: site={scrapeSource}", scrapeSource);

            HashSet<string> imageUrls = new();
            await using SiteParser siteParser = await siteParserFactory.GetParserAsync(scrapeSource);
            using Downloader downloader = downloaderFactory.GetDownloader();

            // Parse the sites images and populate imageUrls
            int parsed = await siteParser.ParseAsync(query, limit, imageUrls, cancellationToken);
            logger.LogInformation("Parsing completed: site='{scrapingSource}', total={parsed}", scrapeSource, parsed);

            // Attempt to download each image
            int downloaded = await downloader.DownloadAsync(outputDirectory, scrapeConfiguration.Format, imageUrls,
                cancellationToken);
            logger.LogInformation("Downloading completed: site='{scrapingSource}', total={parsed}", scrapeSource,
                downloaded);

            return downloaded;
        });

        return (await Task.WhenAll(scrapingTasks)).Sum();
        return 0;
    }
}