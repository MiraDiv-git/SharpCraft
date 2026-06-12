using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Scene;

public static class SceneManager
{
    private static IScene? _currentScene;
    private static bool _initialized = false;

    public static void Initialize()
    {
        Console.WriteLine("[LOAD] Scene Manager initialized.");
    }

    public static void SetScene(IScene scene)
    {
        _currentScene?.Unload();
        _currentScene = scene;
        if (_initialized)
            _currentScene.Load();
    }

    public static void LoadCurrentScene()
    {
        _initialized = true;
        _currentScene?.Load();
    }

    public static void Update() => _currentScene?.Update();
    public static void Render() => _currentScene?.Render();
}