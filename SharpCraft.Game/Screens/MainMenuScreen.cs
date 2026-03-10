using System.Diagnostics;
using System.Runtime.InteropServices;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game.Screens;

public class MainMenuScreen
{
    public static Canvas Canvas { get; private set; }

    private static Sound _clickSound;
    private static Sound _menuLoop;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _logoImage;
    
    public static void Load()
    {
        Canvas = new Canvas(MainMenuScene.UIRenderer);
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _logoImage = AssetManager.LoadTexture(Path.Combine("Textures","UI","Logos","game_logo.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));
        _menuLoop = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "menu_loop.ogg"));
        
        LoadDevButton(); // TODO: Comment before publishing
        LoadPlayButton();
        LoadOptionsButton();
        LoadExitButton();
        LoadCopyrightText();
        LoadLogoImage();
        
        AudioManager.Play(_menuLoop, 30, true);
    }

    private static void LoadDevButton()
    {
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(10, 10);
        rect.Size = new Vector2(32, 32);
        rect.Anchor = Anchor.TopLeft;
        rect.OnClick += () => SceneManager.SetScene(new TestScene());
    }
    
    private static void LoadPlayButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 0);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            Console.WriteLine("[INFO] Changing screen to Play Screen");
            MainMenuScene.SwitchTo(PlayScreen.Canvas);
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "menu.play";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadOptionsButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 50);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            Console.WriteLine("[INFO] Changing screen to Options Screen");
            MainMenuScene.SwitchTo(OptionsScreen.Canvas);
        };
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "menu.options";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadExitButton()
    {
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 100);
        rect.Size = MainMenuScene.defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () => Environment.Exit(0);
        
        // Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "menu.exit";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadCopyrightText()
    {
        var text = Canvas.AddElement<UIText>();
        text.Text = "\u0000 2026 MiraDiv";
        text.Position = new Vector2(-50, -20);
        text.VerticalOffset = -3f;
        text.Anchor = Anchor.BottomRight;
        text.TextColor = Color.White.WithAlpha(220);
        text.FontSize = 12f;
        text.Shadow = false;
        
        var bottomText = Canvas.AddElement<UIText>();
        bottomText.Text = "GPL-3.0 License";
        bottomText.Position = new Vector2(text.Position.X, text.Position.Y + 20);
        bottomText.VerticalOffset = -3f;
        bottomText.Anchor = Anchor.BottomRight;
        bottomText.TextColor = Color.Cyan.WithAlpha(220);
        bottomText.FontSize = 12f;
        bottomText.Shadow = false;
        
        var linkButton = Canvas.AddElement<UIButton>();
        linkButton.Position = new Vector2(0, -12);
        linkButton.Anchor = bottomText.Anchor;
        linkButton.Size = new Vector2(130, 15);
        linkButton.ButtonColor = Color.White.WithAlpha(0);
        linkButton.HoverColor = Color.White.WithAlpha(0);
        linkButton.PressColor = Color.White.WithAlpha(0);
        linkButton.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            OpenUrl("https://www.gnu.org/licenses/gpl-3.0.en.html");
        };
    }

    private static void LoadLogoImage()
    {
        var logo = Canvas.AddElement<UIImage>();
        logo.Position = new Vector2(0, 70);
        logo.Anchor = Anchor.TopCenter;
        logo.Size = new Vector2(1005 / 1.5f, 124 / 1.5f);
        logo.ImageTexture = _logoImage;
    }
    
    private static void OpenUrl(string url)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to open link: {ex.Message}");
        }
    }

    public static void Unload()
    {
        AudioManager.Stop(_menuLoop);
    }
}