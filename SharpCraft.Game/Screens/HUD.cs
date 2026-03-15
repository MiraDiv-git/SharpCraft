using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;
using Silk.NET.Input;

namespace SharpCraft.Game.Screens;

public class HUD
{
    public static Canvas Canvas { get; private set; }
    public static Canvas ChatCanvas { get; private set; }
    public static float CrosshairSize { get; set; } = 10;

    private static Texture _crosshairTexture;
    private static UIImage _crosshair;
    private static UITextField _chatInput;
    private static bool _chatOpen = false;

    public static void Load()
    {
        Canvas = new Canvas(WorldScene.UIRenderer);
        ChatCanvas = new Canvas(WorldScene.UIRenderer);
        
        _crosshairTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "HUD", "crosshair.png"));

        LoadCrosshair();
        LoadChat();
    }
    
    public static void Update()
    {
        _crosshair.Size = new Vector2(CrosshairSize, CrosshairSize);
        if (_chatOpen)
            ChatCanvas.Update(WorldScene.UIRenderer);
    }

    public static void Unload()
    {
        _crosshairTexture.Dispose();
    }

    private static void LoadCrosshair()
    {
        _crosshair = Canvas.AddElement<UIImage>();
        _crosshair.Position = new Vector2(0, 0);
        _crosshair.Size = new Vector2(CrosshairSize, CrosshairSize);
        _crosshair.ImageTexture = _crosshairTexture;
        _crosshair.Anchor = Anchor.MiddleCenter;
        _crosshair.ImageColor = Color.White.WithAlpha(140);
    }
    
    private static void LoadChat()
    {
        _chatInput = ChatCanvas.AddElement<UITextField>();
        _chatInput.Position = new Vector2(0, -20);
        _chatInput.Size = new Vector2(600, 20);
        _chatInput.Anchor = Anchor.BottomCenter;
        _chatInput.FontSize = 9f;
        _chatInput.OnSubmit += text =>
        {
            if (text.Contains("/"))
                CommandHandler.Execute(text);
            CloseChat();
        };
        _chatInput.OnCancel += CloseChat;
    }
    
    public static void OpenChat()
    {
        _chatOpen = true;
        _chatInput.IsFocused = true;
        InputManager.UnlockMouse();
    }
    
    public static void CloseChat()
    {
        _chatOpen = false;
        _chatInput.IsFocused = false;
        InputManager.LockMouse();
    }

    public static bool IsChatOpen => _chatOpen;
}