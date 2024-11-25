using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public sealed class ImageScraper(string imageSource, PageFactory pageFactory, IHttpClientFactory httpClientFactory, ScrapeSettings scrapeSettings)
{
    private readonly BlockingCollection<Uri> _imageUrls = new();
    
    public async Task<int> ScrapeAsync(string query, int limit, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Start scraping image: {imageSource}, {query}");
        
        // Create browser page, parser, and downloader
        IPage page = await pageFactory.CreatePageAsync();
        ImageParser parser = GetImageParser(page);
        ImageDownloader downloader = new ImageDownloader(_imageUrls, imageSource, scrapeSettings, httpClientFactory);
        
        // Parse and download images
        Task<int> parse = parser.ParseImagesAsync(query, limit, cancellationToken);
        Task<int> download = downloader.DownloadImagesAsync(cancellationToken);
        int[] total = await Task.WhenAll(parse, download);
        
        // Return the total images downloaded
        return total[1];
    }

    private ImageParser GetImageParser(IPage page)
    {
        return imageSource switch
        {
            "google" => new GoogleImageParser(_imageUrls, page),
            _ => throw new ArgumentException($"Unsupported image source: {imageSource}")
        };
    }
}