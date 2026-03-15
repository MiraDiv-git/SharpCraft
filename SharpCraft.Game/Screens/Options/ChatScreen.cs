using SharpCraft.Engine;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens.Options;

public class ChatScreen
{
    public static Canvas Canvas { get; private set; }
    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _sliderTexture;
    private static Texture _sliderHandleTexture;
    
    public static UIText ChatSizeText;
    public static UIText ChatMaxMessagesText;
    public static UIText ChatMaxHistoryText;

    public static void Load()
    {
        Canvas = !OptionsScreen.IsGameplay ? new Canvas(MainMenuScene.UIRenderer) : new Canvas(WorldScene.UIRenderer);
        
        if (OptionsScreen.IsGameplay)
        {
            LoadGameplayBackground();
        }
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _sliderTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_background.png"));
        _sliderHandleTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_handle.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));
        
        LoadCategoryText();

        LoadChatSizeSlider();
        LoadChatMaxMessagesSlider();
        LoadChatMaxHistorySlider();
            
        LoadBackButton();
    }
    
    private static void LoadGameplayBackground()
    {
        var bgimage = Canvas.AddElement<UIImage>();
        bgimage.Size = new Vector2(9999, 9999);
        bgimage.Anchor = Anchor.MiddleCenter;
        bgimage.ImageColor = Color.Black.WithAlpha(200);
    }
    
    private static void LoadCategoryText()
    {
        var cattxt = Canvas.AddElement<UIText>();
        cattxt.Text = "options.bind.chat";
        cattxt.Position = new Vector2(0, 20);
        cattxt.Anchor = Anchor.TopCenter;
        cattxt.TextColor = Color.White;
        cattxt.FontSize = 16f;
    }
    
    private static void LoadChatSizeSlider()
    {
        // Vector2 pos = new Vector2(-180, -200);
        Vector2 pos = new Vector2(-180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        ChatSizeText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 8f;
        slider.Max = 48f;
        slider.Step = 8f;
        slider.Value = 24f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            HUD.ChatFontSize = v;
            UserSettings.ChatSize = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.chatsize")}: {v}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.ChatSize;
        HUD.ChatFontSize = slider.Value;
        sText.Text = $"{Localization.Get("options.chatsize")}: {slider.Value}";
    }
    
    private static void LoadChatMaxMessagesSlider()
    {
        Vector2 pos = new Vector2(180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        ChatMaxMessagesText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 3f;
        slider.Max = 60f;
        slider.Step = 1f;
        slider.Value = 20f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            HUD.MaxMessages = (int)v;
            UserSettings.ChatMaxMessages = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.chatmsg")}: {v}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.ChatMaxMessages;
        HUD.MaxMessages = (int)slider.Value;
        sText.Text = $"{Localization.Get("options.chatmsg")}: {slider.Value}";
    }
    
    private static void LoadChatMaxHistorySlider()
    {
        Vector2 pos = new Vector2(-180, -150);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        ChatMaxHistoryText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 50f;
        slider.Max = 500f;
        slider.Step = 1f;
        slider.Value = 50f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            HUD.MaxHistory = (int)v;
            UserSettings.ChatMaxHistory = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.chathist")}: {v}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.ChatMaxHistory;
        HUD.MaxHistory = (int)slider.Value;
        sText.Text = $"{Localization.Get("options.chathist")}: {slider.Value}";
    }
    
    private static void LoadBackButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, -20);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.BottomCenter;
        rect.OnClick += () =>
        {
            Console.WriteLine("[INFO] Changing screen to OptionsScreen");
            
            AudioManager.Play(_clickSound);
            
            if (!OptionsScreen.IsGameplay)
                MainMenuScene.SwitchTo(OptionsScreen.Canvas);
            else
                WorldScene.ChangeScreen(OptionsScreen.Canvas);
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.back";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
}