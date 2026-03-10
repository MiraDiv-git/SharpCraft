using System.Diagnostics;
using System.Runtime.InteropServices;
using SharpCraft.Engine;
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
    private Canvas _playCanvas;
    private Canvas _optionsCanvas;
    private Canvas _loadingCanvas;
    
    private Sound _clickSound;
    private Sound _menuLoop;
    
    private Texture _buttonTexture;
    private Texture _buttonHoverTexture;
    private Texture _smallButtonTexture;
    private Texture _smallButtonHoverTexture;
    private Texture _logoImage;

    private Vector2 defaultButtonSize = new Vector2(350, 40);

    public void Load(UIRenderer uiRenderer)
    {
        Console.WriteLine("[INFO] Loading main menu scene.");
        _uiRenderer = uiRenderer;
        
        _mainCanvas = new Canvas(_uiRenderer);
        _optionsCanvas = new Canvas(_uiRenderer);
        _playCanvas = new Canvas(_uiRenderer);
        _loadingCanvas = new Canvas(_uiRenderer);
        
        _activeCanvas = _mainCanvas; // Canvas that should be shown when loading scene
        // _activeCanvas = _optionsCanvas;
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));
        _menuLoop = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "menu_loop.ogg"));
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _smallButtonTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button.png"));
        _smallButtonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button_hover.png"));
        _logoImage = AssetManager.LoadTexture(Path.Combine("Textures","UI","Logos","game_logo.png"));
        
        LoadMainMenu();
        LoadPlayMenu();
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
        LoadDevButton(); // Comment before publishing
            
        LoadPlayButton();
        LoadOptionsButton();
        LoadExitButton();
        LoadCopyrightText();
        LoadLogoImage();
        AudioManager.Play(_menuLoop, 30, true);
    }

    private void LoadPlayMenu()
    {
        LoadSmallBackButton();
        LoadNewWorldButton();
    }

    private void LoadOptionsMenu()
    {
        LoadBackButton();
        LoadTestLocalizationButtons();
        LoadFPSSlider();
    }
    
    
    //
    // Elements of Main Menu
    //

    private void LoadDevButton()
    {
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(10, 10);
        rect.Size = new Vector2(32, 32);
        rect.Anchor = Anchor.TopLeft;
        rect.OnClick += () => SceneManager.SetScene(new TestScene());
    }
    
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
            Console.WriteLine("[INFO] Changing canvas to Play Canvas");
            SwitchTo(_playCanvas);
        };
        
        // Text
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "menu.play";
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
        text.Text = "menu.options";
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
        text.Text = "menu.exit";
        text.Position = rect.Position;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private void LoadCopyrightText()
    {
        var text = _mainCanvas.AddElement<UIText>();
        text.Text = "\u0000 2026 MiraDiv";
        text.Position = new Vector2(-50, -20);
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

    private void LoadLogoImage()
    {
        var logo = _mainCanvas.AddElement<UIImage>();
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
    
    
    //
    // Elements of Play Menu
    //

    private void LoadSmallBackButton()
    {
        var rect = _playCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(-100, -20);
        rect.Size = new Vector2(defaultButtonSize.X / 2, defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
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
        var text = _playCanvas.AddElement<UIText>();
        text.Text = "play.back";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }

    private void LoadNewWorldButton()
    {
        var rect = _playCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(100, -20);
        rect.Size = new Vector2(defaultButtonSize.X / 2, defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.BottomCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SceneManager.SetScene(new TestWorld());
        };
        
        // Text
        var text = _playCanvas.AddElement<UIText>();
        text.Text = "play.new";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
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
        text.Text = "options.back";
        text.Position = rect.Position;
        text.VerticalOffset = -3f;
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }

    private void LoadFPSSlider()
    {
        Vector2 pos = new Vector2(0, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = _optionsCanvas.AddElement<UIText>();
        sText.Position = pos;
        sText.Anchor = anchor;
        
        var slider = _optionsCanvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 30f;
        slider.Max = 540f;
        slider.Step = 5f;
        slider.Value = 60f;
        slider.BackgroundColor = new Color4(0.3f, 0.3f, 0.3f, 1f);
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(18, defaultButtonSize.Y);
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

    private void LoadTestLocalizationButtons()
    {
        // Categoty text
        var cattxt = _optionsCanvas.AddElement<UIText>();
        cattxt.Text = "options.language";
        cattxt.Position = new Vector2(0, -40);
        cattxt.Anchor = Anchor.MiddleCenter;
        cattxt.TextColor = Color.White;
        cattxt.FontSize = 16f;
        
        // English button
        var buten = _optionsCanvas.AddElement<UIButton>();
        buten.Position = new Vector2(0, 0);
        buten.Size = defaultButtonSize;
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
        var butentxt = _optionsCanvas.AddElement<UIText>();
        butentxt.Text = "English";
        butentxt.Position = buten.Position;
        butentxt.Anchor = buten.Anchor;
        butentxt.TextColor = Color.White;
        butentxt.FontSize = 16f;
        
        // Ukrainian button
        var butua = _optionsCanvas.AddElement<UIButton>();
        butua.Position = new Vector2(0, 50);
        butua.Size = defaultButtonSize;
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
        var butuatxt = _optionsCanvas.AddElement<UIText>();
        butuatxt.Text = "Українська";
        butuatxt.Position = butua.Position;
        butuatxt.Anchor = butua.Anchor;
        butuatxt.TextColor = Color.White;
        butuatxt.FontSize = 16f;
    }
}