using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
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
    public static float ChatFontSize = 16f;
    public static int MaxMessages { get; set; } = 10;
    public static int MaxHistory { get; set; } = 20;

    private static Texture _crosshairTexture;
    private static UIImage _crosshair;
    private static UITextField _chatInput;
    private static bool _chatOpen = false;
    private static readonly List<(string text, float timer)> _messages = new();
    private static float MessageLifetime = 8f;
    private static readonly List<string> _messageHistory = new();
    private static float _chatLineHeight => ChatFontSize * 1.6f;
    
    private static bool _wasChatOpen;

    public static void Load()
    {
        _chatOpen = false;
        
        Canvas = new Canvas(WorldScene.UIRenderer);
        ChatCanvas = new Canvas(WorldScene.UIRenderer);
        
        _crosshairTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "HUD", "crosshair.png"));

        LoadCrosshair();
        LoadChat();
    }
    
    public static void Update()
    {
        _wasChatOpen = _chatOpen;
        _crosshair.Size = new Vector2(CrosshairSize, CrosshairSize);
        if (_chatOpen)
            ChatCanvas.Update(WorldScene.UIRenderer);
        for (int i = _messages.Count - 1; i >= 0; i--)
        {
            var (text, timer) = _messages[i];
            timer -= Time.DeltaTime;
            if (timer <= 0)
                _messages.RemoveAt(i);
            else
                _messages[i] = (text, timer);
        }
    }

    public static void Unload()
    {
        _crosshairTexture.Dispose();
    }
    
    public static void AddMessage(string text)
    {
        _messages.Add((text, MessageLifetime));
        if (_messages.Count > MaxMessages)
            _messages.RemoveAt(0);
        
        _messageHistory.Add(text);
        if (_messageHistory.Count > MaxHistory)
            _messageHistory.RemoveAt(0);
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
    public static bool WasChatOpen => _wasChatOpen;
    
    public static void RenderMessages(UIRenderer renderer, bool forceShowAll = false)
    {
        var toShow = forceShowAll
            ? _messageHistory.TakeLast(MaxHistory).Select(t => (t, 1f)).ToList()
            : _messages.Where(m => m.timer > 0).TakeLast(MaxMessages).ToList();
            
        float lineHeight = _chatLineHeight;
        float startY = renderer.ScreenSize.Y - 50f;

        for (int i = 0; i < toShow.Count; i++)
        {
            var (text, timer) = toShow[i];
            float alpha = (!forceShowAll && timer < 2f) ? timer / 2f : 1f;
            float y = startY - (toShow.Count - i) * lineHeight;

            renderer.DrawRectAbsolute(
                new Vector2(0, y),
                new Vector2(400, lineHeight),
                Color.Black.WithAlpha((byte)(120 * alpha)));

            float x = 4f;
            foreach (var c in text)
            {
                renderer.DrawChar(new Vector2(x, y + (lineHeight - ChatFontSize) / 2f), 
                    ChatFontSize, c, Color.White.WithAlpha((byte)(255 * alpha)));
                // x += (renderer.GetCharWidth(c) + 1) * (_chatFontSize / 8f);
                x += (renderer.GetCharWidth(c) + 0.4f) * (ChatFontSize / 8f);
            }
        }
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
        _chatInput.Position = new Vector2(0, -2);
        _chatInput.Size = new Vector2(800, 20);
        _chatInput.Anchor = Anchor.BottomCenter;
        _chatInput.FontSize = 9f;
        _chatInput.OnSubmit += text =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.StartsWith("/"))
                    CommandHandler.Execute(text);
                else
                    AddMessage(text);
            }
            CloseChat();
        };
        _chatInput.OnCancel += CloseChat;
    }
}