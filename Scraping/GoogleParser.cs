using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public sealed class GoogleParser(IPage page, ILogger<GoogleParser> logger) : SiteParser(page, logger)
{
    private static readonly Regex ImageSourcePattern = new(@"[?&]imgurl=([^&]*)");
    private readonly WaitForSelectorOptions ShortTimeoutOption = new WaitForSelectorOptions { Timeout = 2500 };
    private const string ImageAnchorSelector = @"h3.ob5Hkd > a";
    private const string WaitForHref = @"element => element.getAttribute('href') !== null";
    private const string ExtractHref = @"element => element.getAttribute('href')";
    private const string ScrollDown = @"window.scrollTo(0, document.body.scrollHeight)";
    
    public override async Task<int> ParseAsync(string query, int limit, HashSet<string> imageUrls, 
        CancellationToken cancellationToken)
    {
        // Navigate to google images with the search query
        logger.LogTrace("Navigating to Google, query={query}", query);
        await page.GoToAsync($"https://www.google.com/search?tbm=isch&q={query}");
      
        // Collect all visible image anchor tags
        IElementHandle[] elements = await page.QuerySelectorAllAsync(ImageAnchorSelector);
        
        // When loading images below it caused the anchor tag to unpopulate on a collected handle
        // The solution is to ensure enough images are loaded before started the scraping process
        while (elements.Length < limit)
        {
            await page.EvaluateExpressionAsync(ScrollDown);
            elements = await page.QuerySelectorAllAsync(ImageAnchorSelector);
        }
            
        foreach (IElementHandle element in elements)
        {
            // Hovering over an image populates the anchor tag with an endpoint call that contains the image source
            await element.HoverAsync();

            // Wait for the anchor href to populate
            await page.WaitForFunctionAsync(WaitForHref, element);

            // Extract the endpoint unescape the string
            string endpoint = await element.EvaluateFunctionAsync<string>(ExtractHref);
            endpoint = Uri.UnescapeDataString(endpoint);

            // At this point we don't need this element
            await element.DisposeAsync();

            // Extract the image source and add it to the set
            Match match = ImageSourcePattern.Match(endpoint);

            if (match.Success && imageUrls.Add(match.Groups[1].Value))
            {
                logger.LogTrace("Parsed image source: {source}", match.Groups[1].Value);
            }
            
            // Should we exit
            if (imageUrls.Count >= limit || cancellationToken.IsCancellationRequested) break;
        }

        return imageUrls.Count;
    }
}