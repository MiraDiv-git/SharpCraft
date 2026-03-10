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

    public static void Load()
    {
        Canvas = new Canvas(WorldScene.UIRenderer);

        LoadDebugText();
    }
    
    public static void Update(Camera camera)
    {
        var p = camera.Position;
        _coords.Text = $"Position: {p.X:F2}, {p.Y:F2}, {p.Z:F2}";
        _fps.Text = $"FPS: {(int)(1f / Time.DeltaTime)}";
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
        
        // FPS
        _fps = Canvas.AddElement<UIText>();
        _fps.FontSize = 9f;
        _fps.Anchor = Anchor.TopLeft;
        _fps.Position = new Vector2(_coords.Position.X, _coords.Position.Y + 12);
        _fps.TextColor = Color.White;
        _fps.Shadow = false;
        _fps.Align = TextAlign.Left;
    }
}