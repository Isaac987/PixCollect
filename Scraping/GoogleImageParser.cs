using System.Collections.Concurrent;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public class GoogleImageParser(BlockingCollection<Uri> imageUrls, IPage page) : ImageParser(imageUrls, page)
{
    private readonly BlockingCollection<Uri> _imageUrls = imageUrls;
    private readonly IPage _page = page;
    private readonly HashSet<Uri> _uniqueUrls = new();
    private readonly WaitForSelectorOptions _shortTimeout = new WaitForSelectorOptions { Timeout = 2500 };
    private const string ThumbnailSelector = "g-img.tb08Pd, g-img.mNsIhb";
    private const string ImageSelector = "img.sFlh5c.FyHeAf.iPVvYb";
     
    public override async Task<int> ParseImagesAsync(string query, int limit, CancellationToken cancellationToken)
    {
        await _page.GoToAsync($"https://www.google.com/search?tbm=isch&q={query}");
        
        while (_uniqueUrls.Count < limit && !cancellationToken.IsCancellationRequested)
        {
            IElementHandle[] thumbnails = await ExtractThumbnails();

            foreach (IElementHandle thumbnail in thumbnails)
            {   
                await ExtractImageSource(thumbnail);

                if (_uniqueUrls.Count >= limit || cancellationToken.IsCancellationRequested) break; 
            }
            Console.WriteLine($"Unique Images {_uniqueUrls.Count}");
            await ScrollAndWait();
        }
        
        _imageUrls.CompleteAdding();

        return _uniqueUrls.Count;
    }
    
    private async Task<IElementHandle[]> ExtractThumbnails()
    {
        // Wait for and extract both types of image thumbnail tags
        await _page.WaitForSelectorAsync(ThumbnailSelector);
        return await _page.QuerySelectorAllAsync(ThumbnailSelector);
    }
    
    private async Task ExtractImageSource(IElementHandle thumbnail)
    {
        // Click the thumbnail to expose the uncompressed image
        await _page.EvaluateFunctionAsync("(element) => { element.scrollIntoView(); element.click(); }", thumbnail);
        
        try
        {
            // Wait for the uncompressed image to appear
            IElementHandle image = await _page.WaitForSelectorAsync(ImageSelector, _shortTimeout);
                    
            if (image != null)
            {
                // Get the uncompressed image src from the image
                string src = await _page.EvaluateFunctionAsync<string>("element => element.src", image);
                    
                // Push the url to the shared collection if it has not already been added
                Uri url = new Uri(src);
                if (_uniqueUrls.Add(url)) { _imageUrls.Add(url); }

                // IElementHandle close = await _page.QuerySelectorAsync("uj1Jfd.wv9iH.iM6qI");
                // await _page.EvaluateFunctionAsync("(element) => { element.click(); }", close);
            }
        } catch (WaitTaskTimeoutException)
        {
            // TODO: Implement timeout case for image not loading
        }
    }
    
    private async Task ScrollAndWait()
    {
        await _page.EvaluateExpressionAsync("window.scrollTo(0, document.body.scrollHeight);");
        await Task.Delay(500);
    }
}