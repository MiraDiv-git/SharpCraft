using System.Diagnostics;
using static System.FormattableString;

using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class DebugScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static UIText _coords;
    private static UIText _fps;
    private static UIText _ram;
    private static UIText _blockPos;
    
    private static float _memoryTimer = 0f;
    private static long _lastRamValue = 0;
    private static long _totalRamValue = 0;

    public static void Load()
    {
        Canvas = new Canvas(WorldScene.UIRenderer);
        
        var memInfo = GC.GetGCMemoryInfo();
        _totalRamValue = memInfo.TotalAvailableMemoryBytes / 1024 / 1024;

        LoadDebugText();
    }
    
    public static void Update(Camera camera)
    {
        var p = camera.Position;
        _coords.Text = Invariant($"Position: {p.X:F2},  {p.Y:F2},  {p.Z:F2}");
        _blockPos.Text = $"Block: {Math.Round(p.X, 0)},  {Math.Round(p.Y, 0)},  {Math.Round(p.Z, 0)}";
        _fps.Text = $"FPS: {(int)(1f / Time.DeltaTime)}";
        
        _memoryTimer += (float)Time.DeltaTime;
        if (_memoryTimer >= 0.5f)
        {
            _lastRamValue = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024;
            _memoryTimer = 0;
        }
        _ram.Text = $"RAM: {_lastRamValue} / {_totalRamValue} MB";
    }
    
    private static void LoadDebugText()
    {
        // Coords
        _coords = Canvas.AddElement<UIText>();
        _coords.FontSize = 9f;
        _coords.Anchor = Anchor.TopLeft;
        _coords.Position = new Vector2(5, 0);
        _coords.TextColor = Color.White;
        _coords.Shadow = false;
        _coords.ShadowOffset = 1f;
        _coords.Align = TextAlign.Left;
        
        // Block position
        _blockPos = Canvas.AddElement<UIText>();
        _blockPos.FontSize = 9f;
        _blockPos.Anchor = Anchor.TopLeft;
        _blockPos.Position = new Vector2(_coords.Position.X, _coords.Position.Y + 12);
        _blockPos.TextColor = Color.White;
        _blockPos.Shadow = false;
        _blockPos.ShadowOffset = 1f;
        _blockPos.Align = TextAlign.Left;
        
        // FPS
        _fps = Canvas.AddElement<UIText>();
        _fps.FontSize = 9f;
        _fps.Anchor = Anchor.TopLeft;
        _fps.Position = new Vector2(_blockPos.Position.X, _blockPos.Position.Y + 12);
        _fps.TextColor = Color.White;
        _fps.Shadow = false;
        _fps.Align = TextAlign.Left;
        
        // RAM
        _ram = Canvas.AddElement<UIText>();
        _ram.FontSize = 9f;
        _ram.Anchor = Anchor.TopLeft;
        _ram.Position = new Vector2(_fps.Position.X, _fps.Position.Y + 12);
        _ram.TextColor = Color.White;
        _ram.Shadow = false;
        _ram.Align = TextAlign.Left;
    }
}