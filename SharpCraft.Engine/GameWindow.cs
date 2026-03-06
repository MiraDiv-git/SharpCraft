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
    
    private UIRenderer _uiRenderer;

    public GameWindow()
    {
        _window = Window.Create(WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "SharpCraft",
            UpdatesPerSecond = 60,
            FramesPerSecond = 60
        });

        _window.Load += () =>
        {
            Console.WriteLine("[OK] Game window loaded.");
            _gl = _window.CreateOpenGL();
            Console.WriteLine("[OK] OpenGL context created.");
            Console.WriteLine($"\t↳Using OpenGL: {_gl.GetStringS(StringName.Version)}\n" +
                              $"\t↳Shading Language: {_gl.GetStringS(StringName.ShadingLanguageVersion)}\n" +
                              $"\t↳Renderer: {_gl.GetStringS(StringName.Renderer)}\n" +
                              $"\t↳Vendor: {_gl.GetStringS(StringName.Vendor)}");
            
            _uiRenderer = new UIRenderer(_gl, 800, 600);
            Console.WriteLine("[OK] UI Renderer initialized.");
            
            InputManager.Initialize(_window.CreateInput());
            Console.WriteLine("[OK] Input initialized.");
            
            SceneManager.Initialize(_uiRenderer);
            Console.WriteLine("[OK] SceneManager initialized.");
            SceneManager.LoadCurrentScene();
            Console.WriteLine("\t↳Default scene loaded.");
            
        };
        
        _window.Update += delta =>
        {
            InputManager.Update();
            SceneManager.Update();
        };
        
        _window.Resize += size =>
        {
            if (size.X < 800 || size.Y < 600)
            {
                _window.Size = new Vector2D<int>(Math.Max(size.X, 800), Math.Max(size.Y, 600));
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
    
    public void Run() => _window.Run();
}