using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SharpCraft.Engine.UI;
using Silk.NET.Input;

namespace SharpCraft.Engine;

public class GameWindow : IDisposable
{
    private readonly IWindow _window;
    private GL _gl = null!;
    private RenderServer _server = null!;
    private UIRenderer? _ui;
    private AudioServer? _audio;
    private static GameWindow? _instance;

    private readonly string _font = Config.EngineDefaults.Font.Path;

    public GameWindow() : this(
        Config.EngineDefaults.Window.Title,
        Config.EngineDefaults.Window.Width,
        Config.EngineDefaults.Window.Height)
    {
    }

    public GameWindow(string? title = null, int? width = null, int? height = null)
    {
        _instance = this;

        _window = Window.Create(WindowOptions.Default with
        {
            Size = new Vector2D<int>(
                width ?? Config.EngineDefaults.Window.Width,
                height ?? Config.EngineDefaults.Window.Height),
            Title = title ?? Config.EngineDefaults.Window.Title,
            VSync = Config.EngineDefaults.Window.VSync,
            WindowState = Config.EngineDefaults.Window.Mode,
            WindowBorder = Config.EngineDefaults.Window.Border
        });

        _window.Load += () =>
        {
            PrintMetadata();

            Console.WriteLine("Initializing window...");
            Console.WriteLine("[INIT] Game window loaded.");
            _gl = _window.CreateOpenGL();
            Console.WriteLine("[INIT] OpenGL context created.");
            PrintGLInfo();

            DiscordManager.Initialize();
            Console.WriteLine("[INIT] Discord Rich Presence initialized.");

            _server = new RenderServer(_gl);
            _ui = new UIRenderer(_gl,
                width ?? Config.EngineDefaults.Window.Width,
                height ?? Config.EngineDefaults.Window.Height);
            Console.WriteLine("[INIT] UI Renderer initialized.");
            var refWidth = width ?? Config.EngineDefaults.Window.Width;
            var refHeight = height ?? Config.EngineDefaults.Window.Height;
            Console.WriteLine($"\t└─ Reference resolution: {refWidth}x{refHeight}");

            Console.WriteLine("\nLoading Game Managers...");

            InputManager.Initialize(_window.CreateInput());
            _audio = new AudioServer();

            AssetManager.Initialize(_gl);
            Console.WriteLine($"\t├─ Default font set: {_font}");
            WindowIcon.Set(_window, Path.Combine("Textures", "UI", "Logos", "game_icon.png"));
            Console.WriteLine("\t└─ Window icon set");

            SceneManager.Initialize();
            Console.WriteLine("\t└─ Default scene loaded.");

            Console.WriteLine("\nLoading User Settings...");

            UserSettings.Load();
            KeyBindings.LoadFromSettings();
            SetFPSLock(UserSettings.FPSLock);
            Localization.SetLanguage(UserSettings.Language);

            _ui.SetFont(AssetManager.ReadResourceBytes(_font));

            SceneManager.LoadCurrentScene();

            Console.WriteLine("\n===== Game started =====\n");
        };

        _window.Update += delta =>
        {
            Time.DeltaTime = (float)delta;
            Time.TotalTime += (float)delta;

            InputManager.Update();
            _ui?.Update();
            SceneManager.Update();
        };

        _window.Resize += size =>
        {
            _gl?.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            _ui?.SetScreenSize(size.X, size.Y);
        };

        _window.Render += _ =>
        {
            _server.BeginFrame(Color.DarkGrey);

            SceneManager.Render();

            _server.BeginUI();
            _ui?.Render();
        };

        _window.Closing += Dispose;
    }

    public static void SetFPSLock(double fps)
    {
        _instance!._window.FramesPerSecond = fps;
        _instance._window.UpdatesPerSecond = fps;
        Console.WriteLine($"[FPS] Frame Limit set to {fps}");
    }

    private void PrintGLInfo()
    {
        Console.WriteLine($"\t├─ Using OpenGL: {_gl.GetStringS(StringName.Version)}\n" +
                          $"\t├─ Shading Language: {_gl.GetStringS(StringName.ShadingLanguageVersion)}\n" +
                          $"\t├─ Renderer: {_gl.GetStringS(StringName.Renderer)}\n" +
                          $"\t└─ Vendor: {_gl.GetStringS(StringName.Vendor)}");
    }

    private void PrintMetadata()
    {
        Console.WriteLine($"[INFO] Game: {Config.EngineMetadata.GameInfo.GameName}" +
                          $"\n\t├─ Version: {Config.EngineMetadata.GameInfo.GameVersion}" +
                          $"\n\t├─ Author: {Config.EngineMetadata.GameInfo.GameAuthor}" +
                          $"\n\t└─ Website: {Config.EngineMetadata.GameInfo.GameWebsite}");

        Console.WriteLine($"[INFO] Engine: {Config.EngineMetadata.EngineInfo.EngineName} " +
                          $"{Config.EngineMetadata.EngineInfo.EngineVersion}" +
                          $"\n\t├─ Copyright: {Config.EngineMetadata.EngineInfo.EngineCopyright}" +
                          $"\n\t├─ License: {Config.EngineMetadata.EngineInfo.EngineLicense}" +
                          $"\n\t├─ Website: {Config.EngineMetadata.EngineInfo.EngineWebsite}" +
                          $"\n\t└─ Documentation: {Config.EngineMetadata.EngineInfo.EngineDocs}");

        Console.WriteLine();
    }

    public void Run() => _window.Run();

    public void Dispose()
    {
        SceneManager.Shutdown();
        _ui?.Dispose();
        _ui = null;
        AssetManager.Dispose();
        _audio?.Dispose();
        _audio = null;
        _server?.Dispose();
        _server = null!;
        if (_instance == this)
            _instance = null;
    }
}