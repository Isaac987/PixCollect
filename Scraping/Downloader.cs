using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace PixCollect.Scraping;

public sealed class Downloader(
    ScrapeConfiguration scrapeConfiguration,
    HttpClient httpClient,
    ILogger<Downloader> logger) : IDisposable
{
    public async Task<int> DownloadAsync(HashSet<string> imageUrls, CancellationToken cancellationToken)
    {
        int downloaded = 0;
        
        foreach (string url in imageUrls)
        {
                
        }
        
        return downloaded;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}