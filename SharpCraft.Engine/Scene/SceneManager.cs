using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Scene;

public static class SceneManager
{
    private static IScene? _currentScene;
    private static UIRenderer _uiRenderer;
    private static GL _gl;
    private static bool _initialized = false;

    public static void Initialize(UIRenderer uiRenderer, GL gl)
    {
        _uiRenderer = uiRenderer;
        _gl = gl;
        Console.WriteLine("[OK] Scene Manager initialized.");
    }

    public static void SetScene(IScene scene)
    {
        _currentScene?.Unload();
        _currentScene = scene;
        if (_initialized)
            _currentScene.Load(_uiRenderer, _gl);
    }

    public static void LoadCurrentScene()
    {
        _initialized = true;
        _currentScene?.Load(_uiRenderer, _gl);
    }

    public static void Update() => _currentScene?.Update();
    public static void Render() => _currentScene?.Render();
}