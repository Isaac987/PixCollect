using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public sealed class SiteParserFactory(ILoggerFactory loggerFactory, ILogger<SiteParserFactory> logger) : IDisposable
{
    private IBrowser? _browser;
    
    public async Task<SiteParser> GetParserAsync(string scrapingSource)
    {
        // Get the browser
        _browser ??= await GetBrowser();
        
        // Get the page
        IPage page = await _browser.NewPageAsync();
        
        return scrapingSource switch
        {
            "google" => new GoogleParser(page, loggerFactory.CreateLogger<GoogleParser>()),
            _ => throw new ArgumentException($"Unsupported image source: {scrapingSource}")
        };
    }
    
    public void Dispose()
    {
        _browser?.Dispose();
    }
    
    private async Task<IBrowser> GetBrowser()
    {
        // TODO: check if browser is installed
        logger.LogTrace("Downloading browser");
        await new BrowserFetcher().DownloadAsync();
        
        logger.LogTrace("Launching browser");
        _browser = await Puppeteer.LaunchAsync(new LaunchOptions() { Headless = false });

        return _browser;
    }
}