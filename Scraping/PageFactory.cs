using PuppeteerSharp;

namespace PixCollect.Scraping;

public sealed class PageFactory : IDisposable, IAsyncDisposable
{
    private readonly IBrowser _browser;
    
    private PageFactory(IBrowser browser)
    {
        _browser = browser;
    }

    public void Dispose()
    {
        _browser.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
    }

    public static async Task<PageFactory> CreateAsync()
    {
        // TODO: Check if browser is installed
        // Download the browser executable if absent
        Console.WriteLine("Downloading browser executable...");
        await new BrowserFetcher().DownloadAsync();
        
        IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions() { Headless = false });
        
        return new PageFactory(browser);
    }

    public async Task<IPage> CreatePageAsync()
    {
        return await _browser.NewPageAsync();
    }
}