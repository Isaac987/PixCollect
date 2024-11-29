using System.Text.RegularExpressions;

namespace PixCollect.Scraping;

public static class ScrapeSession
{
    public static string CreateSessionDirectory(string query, string outputDirectory)
    {
        // Use query and timestamp to generate a unique session name
        string sanitizedQuery = SanitizeDirectoryName(query);
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
        string sessionDirectory = $"{sanitizedQuery}_{timestamp}";
        string sessionPath = Path.Combine(outputDirectory, sessionDirectory);
        
        if (!Directory.Exists(sessionPath))
        {
            Directory.CreateDirectory(sessionPath);
        }
        
        return sessionPath;
    }
    
    private static string SanitizeDirectoryName(string directory)
    {
        string invalidPattern = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        directory = directory.Replace(' ', '_');
        directory = Regex.Replace(directory, invalidPattern, string.Empty);
        
        return directory;
    }
}