using Microsoft.Extensions.Logging;
using PixCollect.Configuration;
using SixLabors.ImageSharp;

namespace PixCollect.Scraping;

public sealed class ImageScraper(
    ScrapeConfiguration scrapeConfiguration,
    SiteParserFactory siteParserFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<ImageScraper> logger)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
    
    public async Task<int> ScrapeAsync(string query, int limit, CancellationToken cancellationToken)
    {
        // Create the scraping session directory
        string outputDirectory = ScrapeSession.CreateSessionDirectory(query, scrapeConfiguration.OutputDirectory);
        logger.LogInformation("Created scrape session: directory='{outputDirectory}'", outputDirectory);
        
        IEnumerable<Task<int>> scrapingTasks = scrapeConfiguration.ScrapingSources.Select(async scrapeSource =>
        {
            logger.LogInformation("Starting image scraper: site={scrapeSource}", scrapeSource);

            int count = 0;
            await using SiteParser siteParser = await siteParserFactory.GetParserAsync(scrapeSource);
            
            // Parse each image and download
            await foreach (string url in siteParser.ParseAsync(query, limit).WithCancellation(cancellationToken))
            {
                try
                {
                    await DownloadAsync(url, outputDirectory, cancellationToken);
                    count++;
                }
                catch (UnknownImageFormatException)
                {
                    logger.LogWarning("Failed to download due to unsupported file type: {url}", url);
                }
                catch (HttpRequestException e)
                {
                    logger.LogWarning("Failed to download: {url}, {e}", url, e.Message);
                }
                catch (TaskCanceledException)
                {
                    logger.LogWarning("Failed to download do to timeout: {url}", url);
                }
                
                if (count >= limit || cancellationToken.IsCancellationRequested) break;
            }
            
            return count;
        });

        return (await Task.WhenAll(scrapingTasks)).Sum();
    }

    private async Task DownloadAsync(string url, string outputDirectory, CancellationToken cancellationToken)
    {
        logger.LogTrace("Downloading: {url}", url);
        HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using Image image = await Image.LoadAsync(stream, cancellationToken);
        string filename = Guid.NewGuid().ToString() + '.' + scrapeConfiguration.Format;

        await image.SaveAsync(Path.Combine(outputDirectory, filename), cancellationToken);
        logger.LogTrace("Downloaded image: {filename}", filename);
    }
}