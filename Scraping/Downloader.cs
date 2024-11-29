using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace PixCollect.Scraping;

public sealed class Downloader(
    HttpClient httpClient,
    ILogger<Downloader> logger) : IDisposable
{
    public async Task<int> DownloadAsync(string outputDirectory, string format, HashSet<string> imageUrls,
        CancellationToken cancellationToken)
    {
        int downloaded = 0;
        
        foreach (string url in imageUrls)
        {
            try
            {
                logger.LogTrace("Downloading: {url}", url);
                HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using Image image = await Image.LoadAsync(stream, cancellationToken);
                string filename = Guid.NewGuid().ToString() + '.' + format;

                await image.SaveAsync(Path.Combine(outputDirectory, filename), cancellationToken);
                logger.LogTrace("Downloaded image: {filename}", filename);

                downloaded++;
            }
            catch(HttpRequestException e)
            {
                logger.LogWarning("Failed to download: {url}, {e}", url, e.Message);
            }
            
            if (cancellationToken.IsCancellationRequested) break;
        }

        return downloaded;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}