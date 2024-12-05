using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace PixCollect.Scraping;

public sealed class PixabayParser(IPage page, ILogger<PixabayParser> logger) : SiteParser(page, logger)
{
    private static readonly Regex PageCountPattern = new(@"\d+$");
    private const string BaseUrl = "https://pixabay.com/images/search";
    private const string PageCountSelector = @"div.indicator--Nf9Sc";
    private const string GetPageCount = $@"document.querySelector('{PageCountSelector}').innerText";
    private const string ImageSelector = @"div.container--MwyXl>a>img";
    private const string WaitForSrc = @"element => element.src !== 'https://pixabay.com/static/img/blank.gif'";
    private const string GetImageSrc = @"element => element.src";

    public override async IAsyncEnumerable<string> ParseAsync(string query, int limit)
    {
        // Navigate to pixabay images with the search query
        logger.LogTrace("Navigating to Pixabay, query={query}", query);
        await page.GoToAsync($"{BaseUrl}/{Uri.EscapeDataString(query)}");
        
        // Wait for page count and extract number of pages for this query
        await page.WaitForSelectorAsync(PageCountSelector);
        string pageIndicator = await page.EvaluateExpressionAsync<string>(GetPageCount);
        Match pageCountMatch = PageCountPattern.Match(pageIndicator);
        int pageCount = (pageCountMatch.Success) ? int.Parse(pageCountMatch.Value) : 0;

        for (int i = 2; i <= pageCount; i++)
        {
            IElementHandle[] elements = await page.QuerySelectorAllAsync(ImageSelector);
            
            foreach (IElementHandle element in elements)
            {
                // Look at each image to load its source
                await element.HoverAsync();
                await page.WaitForFunctionAsync(WaitForSrc, element);
                string source = await element.EvaluateFunctionAsync<string>(GetImageSrc);
                
                logger.LogTrace("Parsed image source: {source}", source);
                yield return source;
            }
            
            // Navigate to the next page
            await page.GoToAsync($"{BaseUrl}/{Uri.EscapeDataString(query)}/?pagi={i}");
            
            // TODO: Use a dynamic wait
            await Task.Delay(500);
        }
    }
}