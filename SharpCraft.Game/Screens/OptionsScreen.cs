using SharpCraft.Engine;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI.Elements;
namespace SharpCraft.Game.Screens;

public class OptionsScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _sliderTexture;
    private static Texture _sliderHandleTexture;

    private static bool _isGameplay;

    public static void Load(bool isGameplay = false)
    {
        _isGameplay = isGameplay;
        Canvas = !isGameplay ? new Canvas(MainMenuScene.UIRenderer) : new Canvas(TestWorld.UIRenderer);
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _sliderTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_background.png"));
        _sliderHandleTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_handle.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));

        if (isGameplay)
        {
            LoadGameplayBackground();
        }
        
        LoadBackButton();
        LoadLocalizationButtons();
        LoadFPSSlider();
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
            
            if (!_isGameplay)
            {
                MainMenuScene.SwitchTo(MainMenuScreen.Canvas);
                Console.WriteLine("[INFO] Changing screen to Main Menu Screen");
            }
            else
            {
                TestWorld.ChangeScreen(PauseScreen.Canvas);
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
        Vector2 pos = new Vector2(0, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        sText.Position = pos;
        sText.Anchor = anchor;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 30f;
        slider.Max = 540f;
        slider.Step = 5f;
        slider.Value = 60f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            GameWindow.SetFPSLock(v);
            UserSettings.FPSLock = v;
            UserSettings.Save();
            sText.Text = $"FPS: {v}";
        };
        
        slider.Value = (float)UserSettings.FPSLock;
        GameWindow.SetFPSLock(slider.Value);
        sText.Text = $"FPS: {slider.Value}";
    }

    private static void LoadLocalizationButtons()
    {
        // Categoty text
        var cattxt = Canvas.AddElement<UIText>();
        cattxt.Text = "options.language";
        cattxt.Position = new Vector2(0, -40);
        cattxt.Anchor = Anchor.MiddleCenter;
        cattxt.TextColor = Color.White;
        cattxt.FontSize = 16f;
        
        // English button
        var buten = Canvas.AddElement<UIButton>();
        buten.Position = new Vector2(0, 0);
        buten.Size = MainMenuScene.defaultButtonSize;
        buten.ButtonTexture = _buttonTexture;
        buten.HoverTexture = _buttonHoverTexture;
        buten.ButtonColor = Color.White;
        buten.HoverColor = Color.White;
        buten.Anchor = Anchor.MiddleCenter;
        buten.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            UserSettings.Language = "en";
            UserSettings.Save();
            Localization.SetLanguage("en");
        };
        
        // English text
        var butentxt = Canvas.AddElement<UIText>();
        butentxt.Text = "English";
        butentxt.Position = buten.Position;
        butentxt.Anchor = buten.Anchor;
        butentxt.TextColor = Color.White;
        butentxt.FontSize = 16f;
        
        // Ukrainian button
        var butua = Canvas.AddElement<UIButton>();
        butua.Position = new Vector2(0, 50);
        butua.Size = MainMenuScene.defaultButtonSize;
        butua.ButtonTexture = _buttonTexture;
        butua.HoverTexture = _buttonHoverTexture;
        butua.ButtonColor = Color.White;
        butua.HoverColor = Color.White;
        butua.Anchor = Anchor.MiddleCenter;
        butua.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            UserSettings.Language = "ua";
            UserSettings.Save();
            Localization.SetLanguage("ua");
        };
        
        // Ukrainian text
        var butuatxt = Canvas.AddElement<UIText>();
        butuatxt.Text = "Українська";
        butuatxt.Position = butua.Position;
        butuatxt.Anchor = butua.Anchor;
        butuatxt.TextColor = Color.White;
        butuatxt.FontSize = 16f;
    }
}