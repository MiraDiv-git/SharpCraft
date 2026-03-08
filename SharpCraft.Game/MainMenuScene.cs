using System.Diagnostics;
using System.Runtime.InteropServices;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game;

public class MainMenuScene : IScene
{
    UIRenderer _uiRenderer;
    private Canvas _activeCanvas;
    
    private Canvas _mainCanvas;
    // private Canvas _playCanvas;
    private Canvas _optionsCanvas;
    
    private Sound _clickSound;
    private Sound _menuLoop;
    
    private Texture _buttonTexture;
    private Texture _buttonHoverTexture;
    private Texture _logoImage;

    private Vector2 defaultButtonSize = new Vector2(350, 40);

    public void Load(UIRenderer uiRenderer)
    {
        Console.WriteLine("[INFO] Loading main menu scene.");
        _uiRenderer = uiRenderer;
        
        _mainCanvas = new Canvas(_uiRenderer);
        _optionsCanvas = new Canvas(_uiRenderer);
        
        _activeCanvas = _mainCanvas; // Canvas that should be shown when loading scene
        // _activeCanvas = _optionsCanvas;
        
        _clickSound = AudioManager.LoadAudio("Sounds/UI/click_ui.ogg");
        _menuLoop = AudioManager.LoadAudio("Sounds/UI/menu_loop.ogg");
        
        _buttonTexture = AssetManager.LoadTexture("Textures/UI/Button/button.png");
        _buttonHoverTexture = AssetManager.LoadTexture("Textures/UI/Button/button_hover.png");
        _logoImage = AssetManager.LoadTexture("Textures/UI/Logos/game_logo.png");
        
        LoadMainMenu();
        LoadOptionsMenu();
    }
    
    private void SwitchTo(Canvas canvas)
    {
        _activeCanvas = canvas;
    }

    public void Update() => _activeCanvas.Update(_uiRenderer);
    public void Render() => _activeCanvas.Render();

    public void Unload()
    {
        AudioManager.Stop(_menuLoop);
        _activeCanvas.Clear();
    }

    private void LoadMainMenu()
    {
        LoadPlayButton();
        LoadOptionsButton();
        LoadExitButton();
        LoadCopyrightText();
        LoadLogoImage();
        AudioManager.Play(_menuLoop, 100, true);
    }

    private void LoadOptionsMenu()
    {
        LoadBackButton();
    }
    
    //
    // Elements of Main Menu
    //
    
    private void LoadPlayButton()
    {
        // Button
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 0);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            Console.WriteLine("[INFO] Changing scene to Test Scene.");
            SceneManager.SetScene(new TestScene());
        };
        
        // Text
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "Play";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private void LoadOptionsButton()
    {
        // Button
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 50);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            Console.WriteLine("[INFO] Changing canvas to Options Canvas");
            SwitchTo(_optionsCanvas);
        };
        
        // Text
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "Options";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private void LoadExitButton()
    {
        // Button
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 100);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () => Environment.Exit(0);
        
        // Text
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "Exit";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    //
    // Elements of Options Menu
    //
    
    private void LoadBackButton()
    {
        // Button
        var rect = _optionsCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, -20);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.HoverTexture = _buttonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.BottomCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SwitchTo(_mainCanvas);
            Console.WriteLine("[INFO] Changing canvas to Main Canvas");
        };
        
        // Text
        var text = _optionsCanvas.AddElement<UIText>();
        text.Text = "Apply and back";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }

    private void LoadCopyrightText()
    {
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "\u0000 2026 MiraDiv";
        text.Position = new Vector2(-80, -20);
        text.VerticalOffset = -3f;
        text.Anchor = Anchor.BottomRight;
        text.TextColor = Color.White.WithAlpha(220);
        text.FontSize = 12f;
        text.Shadow = false;
        
        var bottomText = _mainCanvas.AddElement<UIText>();
        bottomText.Text = "GPL-3.0 License";
        bottomText.Position = new Vector2(text.Position.X, text.Position.Y + 20);
        bottomText.VerticalOffset = -3f;
        bottomText.Anchor = Anchor.BottomRight;
        bottomText.TextColor = Color.Cyan.WithAlpha(220);
        bottomText.FontSize = 12f;
        bottomText.Shadow = false;
        
        var linkButton = _mainCanvas.AddElement<UIButton>();
        linkButton.Position = new Vector2(-40, -12);
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

    private void LoadLogoImage()
    {
        var logo = _mainCanvas.AddElement<UIImage>();
        logo.Position = new Vector2(0, 70);
        logo.Anchor = Anchor.TopCenter;
        logo.Size = new Vector2(1005 / 1.5f, 124 / 1.5f);
        logo.ImageTexture = _logoImage;
    }
    
    public static void OpenUrl(string url)
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
    
}