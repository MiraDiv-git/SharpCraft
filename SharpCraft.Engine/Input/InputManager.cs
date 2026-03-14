using Silk.NET.Input;

namespace SharpCraft.Engine.Input;

public class InputManager
{
    public static Vector2 MouseDelta { get; private set; }
    public static bool IsMouseLocked { get; private set; }
    public static Vector2 MousePosition { get; private set; }
    public static bool BlockUIInput { get; set; } = false;
    
    // Hardcode for UI
    private static bool _prevLeftMouseButtonDown;
    public static bool LeftMouseButtonDown => IsMouseButtonDown(MouseButton.Left);
    public static bool LeftMouseButtonJustPressed { get; private set; }

    private static IInputContext _input;
    private static IMouse _mouse;
    private static IKeyboard _keyboard;
    private static Vector2 _lastMousePos;

    private static readonly Dictionary<MouseButton, bool> _mouseButtonsDown = new();
    private static readonly Dictionary<MouseButton, bool> _prevMouseButtonsDown = new();

    private static readonly HashSet<Key> _prevKeys = new();
    private static readonly HashSet<Key> _currKeys = new();
    
    private static bool _prevRightMouseButtonDown;
    public static bool RightMouseButtonDown => IsMouseButtonDown(MouseButton.Right);
    public static bool RightMouseButtonJustPressed { get; private set; }

    public static void Initialize(IInputContext input)
    {
        _input = input;
        _mouse = input.Mice[0];
        _keyboard = input.Keyboards[0];

        _mouse.MouseDown += (_, btn) => _mouseButtonsDown[btn] = true;
        _mouse.MouseUp += (_, btn) => _mouseButtonsDown[btn] = false;

        Console.WriteLine("[OK] Input Manager initialized.");
    }

    public static bool IsMouseButtonDown(MouseButton btn) =>
        _mouseButtonsDown.TryGetValue(btn, out var v) && v;

    public static bool IsMouseButtonJustPressed(MouseButton btn) =>
        _mouseButtonsDown.TryGetValue(btn, out var v) && v &&
        (!_prevMouseButtonsDown.TryGetValue(btn, out var pv) || !pv);

    public static bool IsKeyDown(Key key) => _keyboard.IsKeyPressed(key);
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

    public static void SetCursor(StandardCursor cursor) => _mouse.Cursor.StandardCursor = cursor;
    public static void ResetCursor() => _mouse.Cursor.StandardCursor = StandardCursor.Default;

    public static void Update()
    {
        // Keyboard
        _prevKeys.Clear();
        foreach (var k in _currKeys) _prevKeys.Add(k);
        _currKeys.Clear();
        foreach (var k in Enum.GetValues<Key>())
        {
            if (k == Key.Unknown || (int)k < 0) continue;
            if (_keyboard.IsKeyPressed(k)) _currKeys.Add(k);
        }

        // Mouse position
        var currentPos = new Vector2(_mouse.Position.X, _mouse.Position.Y);
        MouseDelta = IsMouseLocked ? currentPos - _lastMousePos : Vector2.Zero;
        _lastMousePos = currentPos;
        MousePosition = currentPos;

        // UI hardcode
        bool currentLeftDown = IsMouseButtonDown(MouseButton.Left);
        LeftMouseButtonJustPressed = currentLeftDown && !_prevLeftMouseButtonDown;
        _prevLeftMouseButtonDown = currentLeftDown;
        
        _prevMouseButtonsDown.Clear();
        foreach (var btn in _mouseButtonsDown.Keys.ToList())
            _prevMouseButtonsDown[btn] = _mouseButtonsDown[btn];
        
        bool currentRightDown = IsMouseButtonDown(MouseButton.Right);
        RightMouseButtonJustPressed = currentRightDown && !_prevRightMouseButtonDown;
        _prevRightMouseButtonDown = currentRightDown;
    }
}