using Silk.NET.Input;

namespace SharpCraft.Engine.Input;

public struct KeyBind
{
    public Key? Key;
    public MouseButton? Mouse;
    
    public KeyBind(Key key) { Key = key; Mouse = null; }
    public KeyBind(MouseButton mouse) { Key = null; Mouse = mouse; }
    
    public bool IsDown() => Key.HasValue 
        ? InputManager.IsKeyDown(Key.Value) 
        : InputManager.IsMouseButtonDown(Mouse!.Value);
    
    public bool IsJustPressed() => Key.HasValue 
        ? InputManager.IsKeyJustPressed(Key.Value) 
        : InputManager.IsMouseButtonJustPressed(Mouse!.Value);
    
    public override string ToString() => Key.HasValue ? Key.Value.ToString() : Mouse!.Value.ToString();
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
}