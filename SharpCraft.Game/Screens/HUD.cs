using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class HUD
{
    public static Canvas Canvas { get; private set; }
    
    public static float CrosshairSize { get; set; } = 10;

    private static Texture _crosshairTexture;

    public static void Load()
    {
        Canvas = new Canvas(WorldScene.UIRenderer);
        
        _crosshairTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "HUD", "crosshair.png"));

        LoadCrosshair();
    }

    private static void LoadCrosshair()
    {
        var crosshair = Canvas.AddElement<UIImage>();
        crosshair.Position = new Vector2(0, 0);
        crosshair.Size = new Vector2(CrosshairSize, CrosshairSize);
        crosshair.ImageTexture = _crosshairTexture;
        crosshair.Anchor = Anchor.MiddleCenter;
        crosshair.ImageColor = Color.White.WithAlpha(140);
    }
}