using System.Collections.Concurrent;
using SixLabors.ImageSharp;

namespace PixCollect.Scraping;

public class ImageDownloader(BlockingCollection<Uri> imageUrls, string imageSource, ScrapeSettings scrapeSettings, IHttpClientFactory httpClientFactory)
{
    public async Task<int> DownloadImagesAsync(CancellationToken cancellationToken)
    {
        int imageCount = 0;
        using HttpClient httpClient = httpClientFactory.CreateClient();

        System.IO.Directory.CreateDirectory(scrapeSettings.OutputDirectory);
        
        string sourcename = scrapeSettings.Filename.Replace("{s}", imageSource);
        
        foreach (Uri imageUrl in imageUrls.GetConsumingEnumerable(cancellationToken))
        {
            try
            {
                Console.WriteLine($"Downloading image {imageUrl}");
                
                HttpResponseMessage response = await httpClient.GetAsync(imageUrl, cancellationToken);
                response.EnsureSuccessStatusCode();
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                using Image image = Image.Load(imageBytes);
                
                string filename = sourcename.Replace("{i}", imageCount.ToString());
                string outputPath = Path.Combine(scrapeSettings.OutputDirectory, filename);
                
                await image.SaveAsync(outputPath + '.' + scrapeSettings.Format, cancellationToken);
                imageCount++;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error downloading {imageUrl}: {e.Message}");
            }
        }
        
        return imageCount;
    }
}