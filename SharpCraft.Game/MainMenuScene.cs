using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Game.Screens;
using SharpCraft.Game.Screens.Options;

namespace SharpCraft.Game;

public class MainMenuScene : IScene
{
    public static UIRenderer UIRenderer { get; private set; }
    private static Canvas _activeCanvas;

    public static Vector2 defaultButtonSize { get; private set; } = new Vector2(350, 40);

    public void Load(UIRenderer uiRenderer)
    {
        Console.WriteLine("[INFO] Loading main menu scene.");
        UIRenderer = uiRenderer;
        
        MainMenuScreen.Load();
        PlayScreen.Load();
        OptionsScreen.Load();
        
        _activeCanvas = MainMenuScreen.Canvas; // Canvas that should be shown when loading scene
    }
    
    public static void SwitchTo(Canvas canvas)
    {
        _activeCanvas = canvas;
    }

    public void Update()
    {
        _activeCanvas.Update(UIRenderer);
        ControlScreen.Update();
    }

    public void Render() => _activeCanvas.Render();

    public void Unload()
    {
        MainMenuScreen.Unload();
        PlayScreen.Unload();
        OptionsScreen.Unload();
        _activeCanvas.Clear();
        
        _activeCanvas = null;
        UIRenderer = null;
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}