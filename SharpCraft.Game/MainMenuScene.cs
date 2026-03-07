using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
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
        
        _buttonTexture = AssetManager.LoadTexture("Textures/gui/button.png");
        
        LoadMainMenu();
        LoadOptionsMenu();
    }
    
    public void SwitchTo(Canvas canvas)
    {
        _activeCanvas = canvas;
    }

    public void Update() => _activeCanvas.Update(_uiRenderer);
    public void Render() => _activeCanvas.Render();
    public void Unload() => _activeCanvas.Clear();

    private void LoadMainMenu()
    {
        LoadPlayButton();
        LoadOptionsButton();
        LoadExitButton();
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
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 0);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            // Console.WriteLine("[INFO] Play button clicked.");
            Console.WriteLine("[INFO] Changing scene to Test Scene.");
            SceneManager.SetScene(new TestScene());
        };
    }
    
    private void LoadOptionsButton()
    {
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 50);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            // Console.WriteLine("[INFO] Options button clicked.");
            Console.WriteLine("[INFO] Changing canvas to Options Canvas");
            SwitchTo(_optionsCanvas);
        };
    }
    
    private void LoadExitButton()
    {
        var rect = _mainCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 100);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () => Environment.Exit(0);
    }
    
    //
    // Elements of Options Menu
    //
    
    private void LoadBackButton()
    {
        var rect = _optionsCanvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 270);
        rect.Size = defaultButtonSize;
        rect.ButtonTexture = _buttonTexture;
        rect.Anchor = Anchor.MiddleCenter;
        rect.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SwitchTo(_mainCanvas);
            Console.WriteLine("[INFO] Changing canvas to Main Canvas");
        };
    }
    
}