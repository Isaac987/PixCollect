using System.Collections.Concurrent;

namespace PixCollect.Scraping;

public class GoogleImageParser(BlockingCollection<Uri> imageUrls) : ImageParser(imageUrls)
{
    public override async Task<int> ParseImagesAsync(string query, int limit, CancellationToken cancellationToken)
    {
        return 6;
    }
}