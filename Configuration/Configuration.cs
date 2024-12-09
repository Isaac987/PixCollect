using PixCollect.Scraping;

namespace PixCollect.Configuration;

public sealed class Configuration
{
    public required ScrapeConfiguration Scrape { get; set; }
    public required UploadConfiguration Upload { get; set; }
    public required LoggingConfiguration Logging { get; set; }
}