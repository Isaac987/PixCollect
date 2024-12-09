namespace PixCollect.Configuration;

public sealed class ScrapeConfiguration
{
    public string OutputDirectory { get; set; } = "Images";
    public string Format { get; set; } = "jpg";
    public bool Headless { get; set; } = true;
    public List<string> ScrapingSources { get; set; } = ["google"];
}