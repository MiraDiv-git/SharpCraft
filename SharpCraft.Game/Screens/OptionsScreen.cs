using SharpCraft.Engine;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI.Elements;
using SharpCraft.Game.Screens.Options;

namespace SharpCraft.Game.Screens;

public class OptionsScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _sliderTexture;
    private static Texture _sliderHandleTexture;
    
    private static UIText _fpsText;
    private static UIText _crossText;

    public static bool IsGameplay;

    public static void Load(bool isGameplay = false)
    {
        IsGameplay = isGameplay;
        Canvas = !isGameplay ? new Canvas(MainMenuScene.UIRenderer) : new Canvas(WorldScene.UIRenderer);
        LanguageScreen.Load();
        ControlScreen.Load();
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _sliderTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_background.png"));
        _sliderHandleTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_handle.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));

        if (isGameplay)
        {
            LoadGameplayBackground();
        }
        
        LoadFPSSlider();
        LoadCrosshairSlider();
        
        LoadLanguageButton();
        LoadControlsButton();
        
        LoadBackButton();
    }
    
    public static void Unload()
    {
        _buttonTexture.Dispose();
        _buttonHoverTexture.Dispose();
        _sliderTexture.Dispose();
        _sliderHandleTexture.Dispose();
    }
    
    private static void LoadGameplayBackground()
    {
        var bgimage = Canvas.AddElement<UIImage>();
        bgimage.Size = new Vector2(9999, 9999);
        bgimage.Anchor = Anchor.MiddleCenter;
        bgimage.ImageColor = Color.Black.WithAlpha(200);
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
            AudioManager.Play(_clickSound);
            
            if (!IsGameplay)
            {
                MainMenuScene.SwitchTo(MainMenuScreen.Canvas);
                Console.WriteLine("[INFO] Changing screen to Main Menu Screen");
            }
            else
            {
                WorldScene.ChangeScreen(PauseScreen.Canvas);
                Console.WriteLine("[INFO] Changing screen to Pause Screen");
            }
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

    private static void LoadFPSSlider()
    {
        Vector2 pos = new Vector2(180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        _fpsText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 30f;
        slider.Max = 545f;
        slider.Step = 5f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            UserSettings.FPSLock = v > 540f ? 0 : (double)v;
            GameWindow.SetFPSLock(UserSettings.FPSLock);
            UserSettings.Save();
            RefreshTexts();
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = UserSettings.FPSLock <= 0 ? 545f : (float)UserSettings.FPSLock;
        GameWindow.SetFPSLock(UserSettings.FPSLock);
        RefreshTexts();
    }
    
    private static void LoadCrosshairSlider()
    {
        Vector2 pos = new Vector2(-180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        _crossText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 4f;
        slider.Max = 32f;
        slider.Step = 1f;
        slider.Value = 10f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            HUD.CrosshairSize = v;
            UserSettings.CrosshairSize = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.crosssize")}: {v}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.CrosshairSize;
        HUD.CrosshairSize = slider.Value;
        sText.Text = $"{Localization.Get("options.crosssize")}: {slider.Value}";
    }
    
    public static void RefreshTexts()
    {
        if (_fpsText != null)
        {
            string val = UserSettings.FPSLock <= 0
                ? Localization.Get("options.fps.nolimits")
                : UserSettings.FPSLock.ToString();
            _fpsText.Text = $"{Localization.Get("options.fps")}: {val}";
        }
        if (_crossText != null)
            _crossText.Text = $"{Localization.Get("options.crosssize")}: {UserSettings.CrosshairSize}";
    }

    private static void LoadLanguageButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-180, -140);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            if (!IsGameplay)
            {
                MainMenuScene.SwitchTo(LanguageScreen.Canvas);
            }
            else
            {
                WorldScene.ChangeScreen(LanguageScreen.Canvas);
            }
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.language";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }

    private static void LoadControlsButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(180, -140);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            if (!IsGameplay)
            {
                MainMenuScene.SwitchTo(ControlScreen.Canvas);
            }
            else
            {
                WorldScene.ChangeScreen(ControlScreen.Canvas);
            }
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.controls";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
}