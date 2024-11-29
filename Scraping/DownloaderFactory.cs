using Microsoft.Extensions.Logging;

namespace PixCollect.Scraping;

public sealed class DownloaderFactory(
    ScrapeConfiguration scrapeConfiguration,
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory) : IDisposable
{
    public Downloader GetDownloader()
    {
        return new Downloader(scrapeConfiguration, httpClientFactory.CreateClient(),
            loggerFactory.CreateLogger<Downloader>());
    }

    public void Dispose()
    {
    }
}