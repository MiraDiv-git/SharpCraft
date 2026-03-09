namespace SharpCraft.Engine;

public class Localization
{
    public static string CurrentLanguage { get; private set; } = "en";
    
    private static Dictionary<string, string> _strings = new();
    
    public static void Load(string path)
    {
        var json = File.ReadAllText(path);
        _strings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
    }
    
    public static void SetLanguage(string language)
    {
        CurrentLanguage = language;
        string jsonPath = Path.Combine("Localization", $"{language}.json");
        Load(jsonPath);
    }

    public static string Get(string key) => _strings.TryGetValue(key, out var val) ? val : key;
}