using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class PlayScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static Sound _clickSound;
    
    private static Texture _smallButtonTexture;
    private static Texture _smallButtonHoverTexture;
    
    public static void Load()
    {
        Canvas = new Canvas(MainMenuScene.UIRenderer);
        
        _smallButtonTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button.png"));
        _smallButtonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button_hover.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));
        
        LoadSmallBackButton();
        LoadNewWorldButton();
    }

    public static void Unload()
    {
        _smallButtonTexture.Dispose();
        _smallButtonHoverTexture.Dispose();
        Canvas.Clear();
    }
    
    private static void LoadSmallBackButton()
    {
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-100, -20);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.BottomCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            MainMenuScene.SwitchTo(MainMenuScreen.Canvas);
            Console.WriteLine("[INFO] Changing screen to Main Menu Screen");
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "play.back";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }

    private static void LoadNewWorldButton()
    {
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(100, -20);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.BottomCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SceneManager.SetScene(new WorldScene());
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "play.new";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
}