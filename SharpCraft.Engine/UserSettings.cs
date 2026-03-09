namespace SharpCraft.Engine;

public class UserSettings
{
    private const string Path = "settings.json";
    
    public static string Language { get; set; } = "en";
    
    public static void Load()
    {
        if (!File.Exists(Path)) return;
        var json = File.ReadAllText(Path);
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        if (data.TryGetValue("language", out var lang)) Language = lang;
    }
    
    public static void Save()
    {
        var data = new Dictionary<string, string> { ["language"] = Language };
        File.WriteAllText(Path, System.Text.Json.JsonSerializer.Serialize(data));
    }
}