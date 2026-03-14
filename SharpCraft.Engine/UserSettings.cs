using SharpCraft.Engine.Input;

namespace SharpCraft.Engine;

public class UserSettings
{
    private const string Path = "settings.json";
    
    public static string Language { get; set; } = "en";
    public static double FPSLock { get; set; } = 60;
    public static float CrosshairSize { get; set; } = 10;
    public static float Sensitivity { get; set; } = 0.1f;
    public static float FOV { get; set; } = 70f;
    public static string BindMoveForward { get; set; } = "W";
    public static string BindMoveBack { get; set; } = "S";
    public static string BindMoveLeft { get; set; } = "A";
    public static string BindMoveRight { get; set; } = "D";
    public static string BindJump { get; set; } = "Space";
    public static string BindSneak { get; set; } = "ShiftLeft";
    public static string BindPlace { get; set; } = "Left";
    public static string BindDestroy { get; set; } = "Right";
    
    public static void Load()
    {
        if (!File.Exists(Path)) return;
        var json = File.ReadAllText(Path);
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        if (data.TryGetValue("language", out var lang)) Language = lang;
        if (data.TryGetValue("fps_lock", out var fps)) FPSLock = double.Parse(fps);
        if (data.TryGetValue("crosshair_size", out var crosshairSize)) CrosshairSize = float.Parse(crosshairSize);
        if (data.TryGetValue("sensitivity", out var sensitivity)) Sensitivity = float.Parse(sensitivity);
        if (data.TryGetValue("fov", out var fov)) FOV = float.Parse(fov);
        if (data.TryGetValue("bind_forward", out var bf)) BindMoveForward = bf;
        if (data.TryGetValue("bind_back", out var bb)) BindMoveBack = bb;
        if (data.TryGetValue("bind_left", out var bl)) BindMoveLeft = bl;
        if (data.TryGetValue("bind_right", out var br)) BindMoveRight = br;
        if (data.TryGetValue("bind_jump", out var bj)) BindJump = bj;
        if (data.TryGetValue("bind_sneak", out var bs)) BindSneak = bs;
        if (data.TryGetValue("bind_place", out var bp)) BindPlace = bp;
        if (data.TryGetValue("bind_destroy", out var bd)) BindDestroy = bd;
    }
    
    public static void Save()
    {
        var data = new Dictionary<string, string> { ["language"] = Language };
        data["fps_lock"] = FPSLock.ToString();
        data["crosshair_size"] = CrosshairSize.ToString();
        data["sensitivity"] = Sensitivity.ToString();
        data["fov"] = FOV.ToString();
        data["bind_forward"] = KeyBindings.MoveForward.ToString();
        data["bind_back"] = KeyBindings.MoveBack.ToString();
        data["bind_left"] = KeyBindings.MoveLeft.ToString();
        data["bind_right"] = KeyBindings.MoveRight.ToString();
        data["bind_jump"] = KeyBindings.Jump.ToString();
        data["bind_sneak"] = KeyBindings.Sneak.ToString();
        data["bind_place"] = KeyBindings.Place.ToString();
        data["bind_destroy"] = KeyBindings.Destroy.ToString();
        File.WriteAllText(Path, System.Text.Json.JsonSerializer.Serialize(data, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
}