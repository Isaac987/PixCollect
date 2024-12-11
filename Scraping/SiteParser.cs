using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public abstract class SiteParser(IPage page, ILogger<SiteParser> logger) : IAsyncDisposable
{
    public abstract IAsyncEnumerable<string> ParseAsync(string query, int limit);

    public async ValueTask DisposeAsync()
    {
        await page.CloseAsync();
    }
}