using System.Collections.Concurrent;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public abstract class ImageParser(BlockingCollection<Uri> imageUrls, IPage page)
{
    public abstract Task<int> ParseImagesAsync(string query, int limit, CancellationToken cancellationToken);
}