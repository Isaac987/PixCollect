using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public class ImageScraper
{
    private readonly IPage _page;
    private readonly BlockingCollection<Uri> _imageUrls;
    private readonly ImageParser _imageParser;
    private readonly ImageDownloader _imageDownloader;
    
    public ImageScraper(string imageSource, IPage page, ScrapeSettings scrapeSettings)
    {
        _page = page;
        _imageUrls = new BlockingCollection<Uri>();
        _imageParser = GetImageParser(imageSource);
        _imageDownloader = new ImageDownloader(scrapeSettings);
    }
    
    public async Task<int> ScrapeAsync(string query, int limit, CancellationToken cancellationToken)
    {
        // Parse and download images
        Task<int> parse = _imageParser.ParseImagesAsync(query, limit, cancellationToken);
        Task<int> download = _imageDownloader.DownloadImagesAsync(cancellationToken);
        int[] total = await Task.WhenAll(parse, download);
        
        // Return the total images downloaded
        return total[1];
    }

    private ImageParser GetImageParser(string imageSource)
    {
        return imageSource switch
        {
            "google" => new GoogleImageParser(_imageUrls, _page),
            _ => throw new ArgumentException($"Unsupported image source: {imageSource}")
        };
    }
}