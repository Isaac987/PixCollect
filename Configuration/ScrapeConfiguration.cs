namespace PixCollect.Configuration;

public sealed class ScrapeConfiguration
{
    public required string OutputDirectory { get; set; }
    public required string Format { get; set; }
    public required bool Headless { get; set; }
    public required List<string> ScrapingSources { get; set; } = new();
}