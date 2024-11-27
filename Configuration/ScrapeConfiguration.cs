namespace PixCollect.Scraping;

public class ScrapeConfiguration
{
    public string OutputDirectory { get; set; }
    public string Format { get; set; }
    public string Filename { get; set; }
    public string[] ScrapingSources { get; set; }
}