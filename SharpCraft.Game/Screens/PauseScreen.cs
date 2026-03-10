using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class PauseScreen
{
    public static Canvas Canvas { get; private set; }
    
    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    
    public static void Load()
    {
        Canvas = new Canvas(TestWorld.UIRenderer);
        OptionsScreen.Load(true);

        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds","UI","click_ui.ogg"));
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","button_hover.png"));

        LoadBackground();
        LoadResumeButton();
        LoadOptionsButton();
        LoadMenuButton();
    }

    private static void LoadBackground()
    {
        var bgimage = Canvas.AddElement<UIImage>();
        bgimage.Size = new Vector2(9999, 9999);
        bgimage.Anchor = Anchor.MiddleCenter;
        bgimage.ImageColor = Color.Black.WithAlpha(200);
    }

    private static void LoadResumeButton()
    {
        // Button
        var resbut = Canvas.AddElement<UIButton>();
        resbut.Position = new Vector2(0, 0);
        resbut.Size = TestWorld.defaultButtonSize;
        resbut.Anchor = Anchor.MiddleCenter;
        resbut.ButtonColor = Color.White;
        resbut.HoverColor = Color.White;
        resbut.ButtonTexture = _buttonTexture;
        resbut.HoverTexture = _buttonHoverTexture;
        resbut.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            TestWorld.TogglePause(Canvas);
        };
        
        // Text
        var resbuttxt = Canvas.AddElement<UIText>();
        resbuttxt.Position = resbut.Position;
        resbuttxt.Anchor = resbut.Anchor;
        resbuttxt.TextColor = Color.White;
        resbuttxt.Text = "world.pause.resume";
    }

    private static void LoadOptionsButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 50);
        rect.Size = TestWorld.defaultButtonSize;
        rect.Anchor = Anchor.MiddleCenter;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            TestWorld.ChangeScreen(OptionsScreen.Canvas);
        };
        
        // Text
        var txt = Canvas.AddElement<UIText>();
        txt.Position = rect.Position;
        txt.Anchor = rect.Anchor;
        txt.TextColor = Color.White;
        txt.Text = "menu.options";
    }
    
    private static void LoadMenuButton()
    {
        // Button
        var menubutton = Canvas.AddElement<UIButton>();
        menubutton.Position = new Vector2(0, 100);
        menubutton.Size = TestWorld.defaultButtonSize;
        menubutton.Anchor = Anchor.MiddleCenter;
        menubutton.ButtonColor = Color.White;
        menubutton.HoverColor = Color.White;
        menubutton.ButtonTexture = _buttonTexture;
        menubutton.HoverTexture = _buttonHoverTexture;
        menubutton.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SceneManager.SetScene(new MainMenuScene());
        };
        
        // Text
        var menubuttxt = Canvas.AddElement<UIText>();
        menubuttxt.Position = menubutton.Position;
        menubuttxt.Anchor = menubutton.Anchor;
        menubuttxt.TextColor = Color.White;
        menubuttxt.Text = "world.pause.exit";
    }
}