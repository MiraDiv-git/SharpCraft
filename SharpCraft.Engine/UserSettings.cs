namespace SharpCraft.Engine;

public class UserSettings
{
    private const string Path = "settings.json";
    
    public static string Language { get; set; } = "en";
    public static double FPSLock { get; set; } = 60;
    public static float CrosshairSize { get; set; } = 10;
    
    public static void Load()
    {
        if (!File.Exists(Path)) return;
        var json = File.ReadAllText(Path);
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        if (data.TryGetValue("language", out var lang)) Language = lang;
        if (data.TryGetValue("fps_lock", out var fps)) FPSLock = double.Parse(fps);
        if (data.TryGetValue("crosshair_size", out var crosshairSize)) CrosshairSize = float.Parse(crosshairSize);
    }
    
    public static void Save()
    {
        var data = new Dictionary<string, string> { ["language"] = Language };
        data["fps_lock"] = FPSLock.ToString();
        data["crosshair_size"] = CrosshairSize.ToString();
        File.WriteAllText(Path, System.Text.Json.JsonSerializer.Serialize(data, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
}