using SharpCraft.Engine;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens.Options;

public class VideoScreen
{
    public static Canvas Canvas { get; private set; }

    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _sliderTexture;
    private static Texture _sliderHandleTexture;

    public static UIText FOVText;

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
        LoadFOVSlider();
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
        cattxt.Text = "options.video";
        cattxt.Position = new Vector2(0, 20);
        cattxt.Anchor = Anchor.TopCenter;
        cattxt.TextColor = Color.White;
        cattxt.FontSize = 16f;
    }
    
    private static void LoadFOVSlider()
    {
        Vector2 pos = new Vector2(-180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        FOVText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 10f;
        slider.Max = 120f;
        slider.Step = 1f;
        slider.Value = 70f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            Camera.Fov = v;
            UserSettings.FOV = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.fov")}: {v}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.FOV;
        Camera.Fov = slider.Value;
        sText.Text = $"{Localization.Get("options.fov")}: {slider.Value}";
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
            
            if (!OptionsScreen.IsGameplay)
                MainMenuScene.SwitchTo(OptionsScreen.Canvas);
            else
                WorldScene.ChangeScreen(OptionsScreen.Canvas);
            
            Console.WriteLine("[INFO] Changing screen to OptionsScreen");
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