using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class DebugScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static UIText _coords;

    public static void Load()
    {
        Canvas = new Canvas(WorldScene.UIRenderer);

        LoadDebugText();
    }
    
    public static void Update(Camera camera)
    {
        var p = camera.Position;
        _coords.Text = $"Position: {p.X:F2}, {p.Y:F2}, {p.Z:F2}";
    }
    
    private static void LoadDebugText()
    {
        _coords = Canvas.AddElement<UIText>();
        _coords.FontSize = 9f;
        _coords.Anchor = Anchor.TopLeft;
        _coords.Position = new Vector2(5, 0);
        _coords.TextColor = Color.White;
        _coords.Shadow = false;
        _coords.ShadowOffset = 1f;
        _coords.Align = TextAlign.Left;
    }
}