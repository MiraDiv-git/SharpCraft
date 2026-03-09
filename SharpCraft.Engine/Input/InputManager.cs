using Silk.NET.Input;

namespace SharpCraft.Engine.Input;

public class InputManager
{
    public static Vector2 MouseDelta { get; private set; }
    public static bool IsMouseLocked { get; private set; }
    
    private static IInputContext _input;
    private static IMouse _mouse;
    private static IKeyboard _keyboard;
    
    private static bool _prevLeftMouseButtonDown;
    private static Vector2 _lastMousePos;

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
    
    public static bool IsKeyDown(Key key) => _keyboard.IsKeyPressed(key);
    
    private static readonly HashSet<Key> _prevKeys = new();
    private static readonly HashSet<Key> _currKeys = new();
    public static bool IsKeyJustPressed(Key key) => _currKeys.Contains(key) && !_prevKeys.Contains(key);
    
    public static void LockMouse()
    {
        _mouse.Cursor.CursorMode = CursorMode.Raw;
        IsMouseLocked = true;
    }
    
    public static void UnlockMouse()
    {
        _mouse.Cursor.CursorMode = CursorMode.Normal;
        IsMouseLocked = false;
    }
    
    public static void SetCursor(StandardCursor cursor)
    {
        _mouse.Cursor.StandardCursor = cursor;
    }
    
    public static void ResetCursor() => _mouse.Cursor.StandardCursor = StandardCursor.Default;

    public static void Update()
    {
        _prevKeys.Clear();
        foreach (var k in _currKeys) _prevKeys.Add(k);
        _currKeys.Clear();
        foreach (var k in Enum.GetValues<Key>())
            if (_keyboard.IsKeyPressed(k)) _currKeys.Add(k);
        
        var currentPos = new Vector2(_mouse.Position.X, _mouse.Position.Y);
        MouseDelta = IsMouseLocked ? currentPos - _lastMousePos : Vector2.Zero;
        _lastMousePos = currentPos;
        MousePosition = currentPos;
        LeftMouseButtonJustPressed = LeftMouseButtonDown && !_prevLeftMouseButtonDown;
        _prevLeftMouseButtonDown = LeftMouseButtonDown;
    }
    
}