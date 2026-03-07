using SharpCraft.Engine.UI;

namespace SharpCraft.Engine.Scene;

public static class SceneManager
{
    private static IScene? _currentScene;
    private static UIRenderer _uiRenderer;

    public static void Initialize(UIRenderer uiRenderer)
    {
        _uiRenderer = uiRenderer;
        Console.WriteLine("[OK] Scene Manager initialized.");
    }

    public static void SetScene(IScene scene)
    {
        _currentScene?.Unload();
        _currentScene = scene;
    }

    public static void LoadCurrentScene() => _currentScene?.Load(_uiRenderer);
    
    public static void Update() => _currentScene?.Update();
    public static void Render() => _currentScene?.Render();
}