using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PixCollect.Scraping;

public class ImageScraper
{
    private readonly string _imageSource;
    private readonly ScrapeSettings _scrapeSettings;
    private readonly ILogger<ImageScraper> _logger;
    private readonly BlockingCollection<Uri> _imageUrls;
    private readonly ImageParser _imageParser;
    private readonly ImageDownloader _imageDownloader;
    
    public ImageScraper(string imageSource, ScrapeSettings scrapeSettings, ILogger<ImageScraper> logger)
    {
        _imageSource = imageSource;
        _scrapeSettings = scrapeSettings;
        _logger = logger;
        _imageUrls = new BlockingCollection<Uri>();
        _imageParser = GetImageParser();
        _imageDownloader = new ImageDownloader();
    }
    
    public async Task<int> ScrapeAsync(string query, int limit, CancellationToken cancellationToken)
    {
        
        // Parse and download images
        Task<int> parse = _imageParser.ParseImagesAsync(query, limit, cancellationToken);
        Task<int> download = _imageDownloader.DownloadImagesAsync(cancellationToken);
        int[] total = await Task.WhenAll(parse, download);
        
        _logger.LogInformation("Parsed images: total={total}", total[0]);
        _logger.LogInformation("Downloaded images: total={total}", total[1]);
        
        // Return the total images downloaded
        return total[1];
    }

    private ImageParser GetImageParser()
    {
        return _imageSource switch
        {
            "google" => new GoogleImageParser(_imageUrls),
            _ => throw new ArgumentException($"Unsupported image source: {_imageSource}")
        };
    }
}