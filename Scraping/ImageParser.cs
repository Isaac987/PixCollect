using System.Collections.Concurrent;

namespace PixCollect.Scraping;

public abstract class ImageParser(BlockingCollection<Uri> imageUrls)
{
    public abstract Task<int> ParseImagesAsync(string query, int limit, CancellationToken cancellationToken);
}