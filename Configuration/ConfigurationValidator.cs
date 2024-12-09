namespace PixCollect.Configuration;

public static class ConfigurationValidator
{
    // TODO: read from file
    private static readonly HashSet<string> Sources = ["google", "pixabay"];
    private static readonly HashSet<string> Formats = ["bmp", "gif", "jpeg", "jpg", "pbm", "png", "tiff", "tif", "tga", "webp"];
    
    public static bool IsValidSource(string source)
    {
        return Sources.Contains(source);
    }

    public static bool IsValidFormat(string format)
    {
        return Formats.Contains(format);
    }
    
}