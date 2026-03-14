using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens.Options;

public class ControlScreen
{
    public static Canvas Canvas { get; private set; }

    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;

    public static void Load()
    {
        Canvas = !OptionsScreen.IsGameplay ? new Canvas(MainMenuScene.UIRenderer) : new Canvas(WorldScene.UIRenderer);
        
        if (OptionsScreen.IsGameplay)
        {
            LoadGameplayBackground();
        }
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));

        LoadCategoryText();
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
        cattxt.Text = "options.controls";
        cattxt.Position = new Vector2(0, 20);
        cattxt.Anchor = Anchor.TopCenter;
        cattxt.TextColor = Color.White;
        cattxt.FontSize = 16f;
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
            {
                MainMenuScene.SwitchTo(OptionsScreen.Canvas);
            }
            else
            {
                WorldScene.ChangeScreen(OptionsScreen.Canvas);
            }
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