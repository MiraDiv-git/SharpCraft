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

public class GameWindow
{
    private readonly IWindow _window;
    private GL _gl = null!;
    private static GameWindow? _instance;

    private readonly string _font = Config.EngineDefaults.Font.Path;

    public GameWindow()
    {
        _instance = this;
        
        _window = Window.Create(WindowOptions.Default with
        {
            Size = new Vector2D<int>(
                Config.EngineDefaults.Window.Width, 
                Config.EngineDefaults.Window.Height),
            Title = Config.EngineDefaults.Window.Title,
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
            
            _ = new UIRenderer(_gl, 
                Config.EngineDefaults.Window.Width,
                Config.EngineDefaults.Window.Height);
            Console.WriteLine("[INIT] UI Renderer initialized.");
            Console.WriteLine($"\t└─ Reference resolution: {Config.EngineDefaults.Window.Width.ToString()}x" +
                              $"{Config.EngineDefaults.Window.Height.ToString()}");
            
            Console.WriteLine("\nLoading Game Managers...");
            
            InputManager.Initialize(_window.CreateInput());
            AudioManager.Initialize();
            
            AssetManager.Initialize(_gl);
            Console.WriteLine($"\t├─ Default font set: {_font}");
            WindowIcon.Set(_window, Path.Combine("Textures", "UI", "Logos", "game_icon.png"));
            Console.WriteLine("\t└─ Window Icon set");
            
            SceneManager.Initialize();
            Console.WriteLine("\t└─ Default scene loaded.");
            
            Console.WriteLine("\nLoading User Settings...");
            
            UserSettings.Load();
            KeyBindings.LoadFromSettings();
            SetFPSLock(UserSettings.FPSLock);
            Localization.SetLanguage(UserSettings.Language);
            
            var (texture, pixels, w, h) = AssetManager.LoadFontTexture(_font);
            UIRenderer.Instance!.SetFont(texture, pixels, w, h);
            
            SceneManager.LoadCurrentScene();
            
            Console.WriteLine("\n===== Game started =====\n");
        };
        
        _window.Update += delta =>
        {
            Time.DeltaTime = (float)delta;
            Time.TotalTime += (float)delta;
            
            InputManager.Update();
            UIRenderer.Instance!.Update();
            SceneManager.Update();
        };
        
        _window.Resize += size =>
        {
            if (size.X < Config.EngineDefaults.Window.Width || 
                size.Y < Config.EngineDefaults.Window.Height)
            {
                _window.Size = new Vector2D<int>(Math.Max(size.X, Config.EngineDefaults.Window.Width), 
                    Math.Max(size.Y, Config.EngineDefaults.Window.Height));
                return;
            }
            
            _gl!.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            UIRenderer.Instance!.SetScreenSize(size.X, size.Y);
        };
        
        _window.Render += delta =>
        {
            var c = Color.DarkGrey;
            _gl!.ClearColor(c.r, c.g, c.b, c.a);
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            SceneManager.Render();
        };
    }
    
    private static void SetFPSLock(double fps)
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
        Console.WriteLine($"==== {Config.EngineDefaults.Window.Title} =====");
        Console.WriteLine();
        
        Console.WriteLine($"[INFO] Engine: {Config.EngineMetadata.Info.EngineName} " +
                          $"{Config.EngineMetadata.Info.EngineVersion}" +
                          $"\n\t├─ Copyright: {Config.EngineMetadata.Info.EngineCopyright}" +
                          $"\n\t├─ License: {Config.EngineMetadata.Info.License}" +
                          $"\n\t├─ Website: {Config.EngineMetadata.Info.Website}" +
                          $"\n\t└─ Documentation: {Config.EngineMetadata.Info.Docs}");
        
        Console.WriteLine($"[INFO] Game: {Config.EngineMetadata.Info.GameName}" +
                          $"\n\t└─ Author: {Config.EngineMetadata.Info.GameAuthor}");
        
        Console.WriteLine();
    }
    
    
    public void Run() => _window.Run();
}