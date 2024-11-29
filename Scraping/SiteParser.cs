using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public abstract class SiteParser(IPage page, ILogger<SiteParser> logger) : IAsyncDisposable
{
    public abstract Task<int> ParseAsync(string query, int limit, HashSet<string> imageUrls, CancellationToken cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await page.CloseAsync();
    }
}