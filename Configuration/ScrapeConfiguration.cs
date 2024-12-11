namespace PixCollect.Configuration;

public sealed class ScrapeConfiguration
{
    public string OutputDirectory { get; set; } 
    public string Format { get; set; }
    public bool Headless { get; set; }
    public List<string> ScrapingSources { get; set; }
}