using Microsoft.Extensions.Logging;

namespace PixCollect.Scraping;

public sealed class DownloaderFactory(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IDisposable
{
    public Downloader GetDownloader()
    {
        return new Downloader(httpClient,
            loggerFactory.CreateLogger<Downloader>());
    }

    public void Dispose()
    {
    }
}