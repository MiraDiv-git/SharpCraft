using Silk.NET.Input;

namespace SharpCraft.Engine.Input;

public struct KeyBind
{
    public Key? KeyCode;
    public MouseButton? Mouse;
    
    public KeyBind(Key key) { KeyCode = key; Mouse = null; }
    public KeyBind(MouseButton mouse) { KeyCode = null; Mouse = mouse; }
    
    public bool IsDown() => KeyCode.HasValue 
        ? InputManager.IsKeyDown(KeyCode.Value) 
        : InputManager.IsMouseButtonDown(Mouse!.Value);
    
    public bool IsJustPressed() => KeyCode.HasValue 
        ? InputManager.IsKeyJustPressed(KeyCode.Value) 
        : Mouse == MouseButton.Left ? InputManager.LeftMouseButtonJustPressed
            : Mouse == MouseButton.Right ? InputManager.RightMouseButtonJustPressed
                : InputManager.IsMouseButtonJustPressed(Mouse!.Value);
    
    public override string ToString() => KeyCode.HasValue ? KeyCode.Value.ToString() : Mouse!.Value.ToString();
    
    public static KeyBind FromString(string value)
    {
        if (Enum.TryParse<MouseButton>(value, out var mouse))
            return new KeyBind(mouse);
        if (Enum.TryParse<Key>(value, out var key))
            return new KeyBind(key);
        return new KeyBind(Key.Unknown);
    }
}

public static class KeyBindings
{
    public static KeyBind MoveForward = new(Key.W);
    public static KeyBind MoveBack = new(Key.S);
    public static KeyBind MoveLeft = new(Key.A);
    public static KeyBind MoveRight = new(Key.D);
    public static KeyBind Jump = new(Key.Space);
    public static KeyBind Sprint = new(Key.ControlLeft);
    public static KeyBind Sneak = new(Key.ShiftLeft);
    public static KeyBind Destroy = new(MouseButton.Left);
    public static KeyBind Place = new(MouseButton.Right);
    
    public static void LoadFromSettings()
    {
        MoveForward = KeyBind.FromString(UserSettings.BindMoveForward);
        MoveBack = KeyBind.FromString(UserSettings.BindMoveBack);
        MoveLeft = KeyBind.FromString(UserSettings.BindMoveLeft);
        MoveRight = KeyBind.FromString(UserSettings.BindMoveRight);
        Jump = KeyBind.FromString(UserSettings.BindJump);
        Sneak = KeyBind.FromString(UserSettings.BindSneak);
        Place = KeyBind.FromString(UserSettings.BindPlace);
        Destroy = KeyBind.FromString(UserSettings.BindDestroy);
    }
}