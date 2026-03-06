using Silk.NET.Input;

namespace SharpCraft.Engine.Input;

public class InputManager
{
    private static IInputContext _input;
    private static IMouse _mouse;
    private static IKeyboard _keyboard;
    private static bool _prevLeftMouseButtonDown;

    public static bool LeftMouseButtonJustPressed { get; private set; }
    public static Vector2 MousePosition { get; private set; }
    public static bool LeftMouseButtonDown { get; private set; }

    public static void Initialize(IInputContext input)
    {
        _input = input;
        _mouse = input.Mice[0];
        _keyboard = input.Keyboards[0];
        
        _mouse.MouseDown += (_, btn) => { if (btn == MouseButton.Left) LeftMouseButtonDown = true; };
        _mouse.MouseUp += (_, btn) => { if (btn == MouseButton.Left) LeftMouseButtonDown = false; };
        
        Console.WriteLine("[OK] Input Manager initialized.");
    }

    public static void Update()
    {
        MousePosition = new Vector2(_mouse.Position.X, _mouse.Position.Y);
        LeftMouseButtonJustPressed = LeftMouseButtonDown && !_prevLeftMouseButtonDown;
        _prevLeftMouseButtonDown = LeftMouseButtonDown;
    }
}