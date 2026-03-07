using SharpCraft.Engine.Input;

namespace SharpCraft.Engine.UI.Elements;

public enum ButtonState
{
    Normal,
    Pressed,
    Hovered
}

public class UIButton : UIElement
{
    public Color4 ButtonColor { get; set; } = Color.Grey;
    public Color4 HoverColor { get; set; } = Color.LightGrey;
    public Color4 PressColor { get; set; } = Color.White;
    public Texture? ButtonTexture { get; set; }
    public Texture? HoverTexture { get; set; }
    public Texture? PressTexture { get; set; }
    
    public Action? OnClick { get; set; }
    public Action? OnHover { get; set; }
    
    public ButtonState State { get; private set; } = ButtonState.Normal;

    public override void Update(UIRenderer renderer)
    {
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
        var mouse = InputManager.MousePosition;
        
        bool hovered = mouse.X >= resolvedPos.X && mouse.X <= resolvedPos.X + resolvedSize.X &&
                       mouse.Y >= resolvedPos.Y && mouse.Y <= resolvedPos.Y + resolvedSize.Y;

        if (hovered)
        {
            if (InputManager.LeftMouseButtonDown)
            {
                State = ButtonState.Pressed;
                if (InputManager.LeftMouseButtonJustPressed)
                    OnClick?.Invoke();
            }
            else
            {
                State = ButtonState.Hovered;
                OnHover?.Invoke();
            }
        }
        else
        {
            State = ButtonState.Normal;
        }
    }
    
    public override void Render(UIRenderer renderer)
    {
        var color = State switch
        {
            ButtonState.Hovered => HoverColor,
            ButtonState.Pressed => PressColor,
            _ => ButtonColor
        };

        var texture = State switch
        {
            ButtonState.Hovered => HoverTexture ?? ButtonTexture,
            ButtonState.Pressed => PressTexture ?? ButtonTexture,
            _ => ButtonTexture
        };
        
        if (ButtonTexture != null)
            renderer.DrawTexturedRect(Position, Size, texture, color, Anchor);
        else
            renderer.DrawRect(Position, Size, color, Anchor);
    }
}