using SharpCraft.Engine.Input;

namespace SharpCraft.Engine.UI;

public enum ButtonState
{
    Normal,
    Pressed,
    Hovered
}

public enum ClickMode
{
    OnPress,
    OnRelease
}

public class UIButton : UIElement
{
    public Color4 NormalColor { get; set; } = Color.Grey;
    public Color4 HoverColor { get; set; } = Color.LightGrey;
    public Color4 PressColor { get; set; } = Color.White;
    public Texture? NormalTexture { get; set; }
    public Texture? HoverTexture { get; set; }
    public Texture? PressTexture { get; set; }
    
    public Action? OnClick { get; set; }
    public Action? OnHover { get; set; }
    public ButtonState State { get; private set; } = ButtonState.Normal;
    public ClickMode ClickMode { get; set; } = ClickMode.OnRelease;
    
    private readonly UIText _buttonText = new();
    public string Text { get => _buttonText.Text; set => _buttonText.Text = value; }
    public TextAlign TextAlign { get => _buttonText.Align; set => _buttonText.Align = value; }
    public float FontSize { get => _buttonText.FontSize; set => _buttonText.FontSize = value; }
    public Color4 FontColor { get => _buttonText.FontColor; set => _buttonText.FontColor = value; }
    public float FontSpacing { get => _buttonText.Spacing; set => _buttonText.Spacing = value; }
    public bool FontShadow { get => _buttonText.Shadow; set => _buttonText.Shadow = value; }
    public float FontShadowOffset { get => _buttonText.ShadowOffset; set => _buttonText.ShadowOffset = value; }

    public override void Update(UIRenderer renderer)
    {
        if (InputManager.BlockUIInput) return;
        
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
        var mouse = InputManager.MousePosition;
        
        bool hovered = mouse.X >= resolvedPos.X && mouse.X <= resolvedPos.X + resolvedSize.X &&
                       mouse.Y >= resolvedPos.Y && mouse.Y <= resolvedPos.Y + resolvedSize.Y;

        if (hovered)
        {
            // if (!OperatingSystem.IsWindows())
            //     InputManager.SetCursor(StandardCursor.Hand);
    
            if (InputManager.LeftMouseButtonDown)
            {
                State = ButtonState.Pressed;
                if (ClickMode == ClickMode.OnPress && InputManager.LeftMouseButtonJustPressed)
                    OnClick?.Invoke();
            }
            else
            {
                if (ClickMode == ClickMode.OnRelease && InputManager.LeftMouseButtonJustReleased)
                    OnClick?.Invoke();
        
                State = ButtonState.Hovered;
                OnHover?.Invoke();
            }
        }
        else
        {
            if (!InputManager.LeftMouseButtonDown)
                State = ButtonState.Normal;
        }
    }
    
    public override void Render(UIRenderer renderer)
    {
        var color = State switch
        {
            ButtonState.Hovered => HoverColor,
            ButtonState.Pressed => PressColor,
            _ => NormalColor
        };

        var texture = State switch
        {
            ButtonState.Hovered => HoverTexture ?? NormalTexture,
            ButtonState.Pressed => PressTexture ?? NormalTexture,
            _ => NormalTexture
        };
        
        if (NormalTexture != null)
            renderer.DrawTexturedRect(Position, Size, texture!, color, Anchor);
        else
            renderer.DrawRect(Position, Size, color, Anchor);

        if (!string.IsNullOrEmpty(Text))
        {
            _buttonText.Position = this.Position;
            _buttonText.Size = this.Size;
            _buttonText.Anchor = this.Anchor;
            _buttonText.VerticalOffset = State == ButtonState.Pressed ? 1f : 0f;

            _buttonText.Render(renderer);
        }
    }
}