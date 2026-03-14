using SharpCraft.Engine;
using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;
using Silk.NET.Input;

namespace SharpCraft.Game.Screens.Options;

public class ControlScreen
{
    public static Canvas Canvas { get; private set; }
    public static UIText SensText;

    private static Sound _clickSound;
    
    private static Texture _buttonTexture;
    private static Texture _buttonHoverTexture;
    private static Texture _smallButtonTexture;
    private static Texture _smallButtonHoverTexture;
    private static Texture _sliderTexture;
    private static Texture _sliderHandleTexture;
    
    private static bool _waitingForRelease = false;
    private static bool _waitingForBind = false;
    private static Action<KeyBind> _onBindReceived;

    public static void Load()
    {
        Canvas = !OptionsScreen.IsGameplay ? new Canvas(MainMenuScene.UIRenderer) : new Canvas(WorldScene.UIRenderer);
        
        if (OptionsScreen.IsGameplay)
        {
            LoadGameplayBackground();
        }
        
        _buttonTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button.png"));
        _buttonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Button", "button_hover.png"));
        _smallButtonTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button.png"));
        _smallButtonHoverTexture = AssetManager.LoadTexture(Path.Combine("Textures","UI","Button","Small","small_button_hover.png"));
        _sliderTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_background.png"));
        _sliderHandleTexture = AssetManager.LoadTexture(Path.Combine("Textures", "UI", "Slider", "slider_handle.png"));
        
        _clickSound = AudioManager.LoadAudio(Path.Combine("Sounds", "UI", "click_ui.ogg"));

        LoadCategoryText();
        
        LoadSensitivitySlider();
        LoadForwardBindButton();
        LoadBackwardBindButton();
        LoadLeftBindButton();
        LoadRightBindButton();
        LoadJumpBindButton();
        LoadSneakBindButton();
        
        LoadPlaceBindButton();
        LoadDestroyBindButton();
        
        LoadBackButton();
    }

    public static void Update()
    {
        if (!_waitingForBind) return;
        if (_waitingForRelease)
        {
            if (!InputManager.LeftMouseButtonDown)
                _waitingForRelease = false;
            return;
        }

        foreach (Key key in Enum.GetValues<Key>())
        {
            if (key == Key.Unknown || (int)key < 0) continue;
            if (InputManager.IsKeyJustPressed(key))
            {
                _onBindReceived?.Invoke(new KeyBind(key));
                _waitingForBind = false;
                InputManager.BlockUIInput = false;
                return;
            }
        }
        
        if (InputManager.LeftMouseButtonJustPressed)
        {
            _onBindReceived?.Invoke(new KeyBind(MouseButton.Left));
            _waitingForBind = false;
            InputManager.BlockUIInput = false;
            return;
        }
        if (InputManager.RightMouseButtonJustPressed)
        {
            _onBindReceived?.Invoke(new KeyBind(MouseButton.Right));
            _waitingForBind = false;
            InputManager.BlockUIInput = false;
            return;
        }
    }
    
    private static void StartListening(Action<KeyBind> onBind)
    {
        _waitingForBind = true;
        _waitingForRelease = true;
        _onBindReceived = onBind;
        InputManager.BlockUIInput = true;
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
    
    private static void LoadSensitivitySlider()
    {
        Vector2 pos = new Vector2(-180, -200);
        Anchor anchor = Anchor.MiddleCenter;
        
        var sText = Canvas.AddElement<UIText>();
        SensText = sText;
        
        var slider = Canvas.AddElement<UISlider>();
        slider.Position = pos;
        slider.Size = MainMenuScene.defaultButtonSize;
        slider.Anchor = anchor;
        slider.Min = 0.01f;
        slider.Max = 1f;
        slider.Step = 0.01f;
        slider.Value = 0.08f;
        slider.BackgroundTexture = _sliderTexture;
        slider.HandleTexture = _sliderHandleTexture;
        slider.BackgroundColor = Color.White;
        slider.HandleColor = Color.White;
        slider.HandleSize = new Vector2(23, MainMenuScene.defaultButtonSize.Y);
        slider.OnValueChanged += v =>
        {
            Camera.Sensitivity = v;
            UserSettings.Sensitivity = v;
            UserSettings.Save();
            sText.Text = $"{Localization.Get("options.sensitivity")}: {Math.Round(v * 100, 0)}";
        };
        
        sText.Position = pos;
        sText.Anchor = anchor;
        sText.Size = slider.Size;
        
        slider.Value = (float)UserSettings.Sensitivity;
        Camera.Sensitivity = slider.Value;
        sText.Text = $"{Localization.Get("options.sensitivity")}: {Math.Round(slider.Value * 100, 0)}";
    }
    
    private static void LoadForwardBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.MoveForward.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, -100);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.MoveForward = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.forward";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadBackwardBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.MoveBack.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, -50);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.MoveBack = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.back";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadLeftBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.MoveLeft.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, 0);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.MoveLeft = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.left";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadRightBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.MoveRight.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, 50);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.MoveRight = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.right";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadJumpBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.Jump.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, 100);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.Jump = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.jump";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadSneakBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.Sneak.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(-150, 150);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.Sneak = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.sneak";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadPlaceBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.Place.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(150, -100);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.Place = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.place";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
    }
    
    private static void LoadDestroyBindButton()
    {
        // Button Text
        var bText = Canvas.AddElement<UIText>();
        bText.Text = KeyBindings.Destroy.ToString();
        
        // Button
        var rect = Canvas.AddElement<UIButton>();
        rect.Position = new Vector2(150, -50);
        rect.Size = new Vector2(MainMenuScene.defaultButtonSize.X / 2, MainMenuScene.defaultButtonSize.Y);
        rect.ButtonTexture = _smallButtonTexture;
        rect.HoverTexture = _smallButtonHoverTexture;
        rect.ButtonColor = Color.White;
        rect.HoverColor = Color.White;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            bText.Text = "...";
            StartListening(bind =>
            {
                KeyBindings.Destroy = bind;
                bText.Text = bind.ToString();
                UserSettings.Save();
            });
        };
        
        // Button text position
        bText.Position = rect.Position;
        bText.Anchor = rect.Anchor;
        bText.TextColor = Color.White;
        bText.FontSize = 16f;
        
        // Bind Text
        var text = Canvas.AddElement<UIText>();
        text.Text = "options.bind.destroy";
        text.Position = new Vector2(rect.Position.X - 143, rect.Position.Y);
        text.Anchor = rect.Anchor;
        text.TextColor = Color.White;
        text.FontSize = 16f;
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