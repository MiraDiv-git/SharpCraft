using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Input;
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
    private GL _gl;

    private readonly int defaultWindowWidth = 800;
    private readonly int defaultWindowHeight = 600;
    private readonly string defaultFont = "Fonts/ifeelnostalgic.png";
    private UIRenderer _uiRenderer;

    public GameWindow()
    {
        _window = Window.Create(WindowOptions.Default with
        {
            Size = new Vector2D<int>(defaultWindowWidth, defaultWindowHeight),
            Title = "SharpCraft",
            UpdatesPerSecond = 60,
            FramesPerSecond = 60
        });

        _window.Load += () =>
        {
            Console.WriteLine("[OK] Game window loaded.");
            _gl = _window.CreateOpenGL();
            Console.WriteLine("[OK] OpenGL context created.");
            PrintGLInfo();
            
            _uiRenderer = new UIRenderer(_gl, defaultWindowWidth, defaultWindowHeight);
            Console.WriteLine("[OK] UI Renderer initialized.");
            
            Console.WriteLine("\nLoading Game Managers...");
            
            InputManager.Initialize(_window.CreateInput());
            AudioManager.Initialize();
            AssetManager.Initialize(_gl);
            
            var (texture, pixels, w, h) = AssetManager.LoadFontTexture(defaultFont);
            _uiRenderer.SetFont(texture, pixels, w, h);
            Console.WriteLine($"\t↳Default font set: {defaultFont}");
            
            SceneManager.Initialize(_uiRenderer);
                SceneManager.LoadCurrentScene();
                Console.WriteLine("\t↳Default scene loaded.");
                
            Console.WriteLine("\n===== Game started =====\n");
        };
        
        _window.Update += delta =>
        {
            InputManager.Update();
            SceneManager.Update();
        };
        
        _window.Resize += size =>
        {
            if (size.X < defaultWindowWidth || size.Y < defaultWindowHeight)
            {
                _window.Size = new Vector2D<int>(Math.Max(size.X, defaultWindowWidth), 
                    Math.Max(size.Y, defaultWindowHeight));
                return;
            }
            
            if (_uiRenderer == null) return;
            _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            _uiRenderer.SetScreenSize(size.X, size.Y);
            // Console.WriteLine($"[INFO] Resized to {size.X}x{size.Y}");
        };
        
        _window.Render += delta =>
        {
            var c = Color.DarkGrey;
            _gl.ClearColor(c.r, c.g, c.b, c.a);
            _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            SceneManager.Render();
        };
    }
    
    private void PrintGLInfo()
    {
        Console.WriteLine($"\t↳Using OpenGL: {_gl.GetStringS(StringName.Version)}\n" +
                          $"\t↳Shading Language: {_gl.GetStringS(StringName.ShadingLanguageVersion)}\n" +
                          $"\t↳Renderer: {_gl.GetStringS(StringName.Renderer)}\n" +
                          $"\t↳Vendor: {_gl.GetStringS(StringName.Vendor)}");
    }
    
    public void Run() => _window.Run();
}