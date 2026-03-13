using SharpCraft.Engine.Input;

namespace SharpCraft.Engine.UI.Elements;

public class UISlider : UIElement
{
    public Texture? BackgroundTexture { get; set; }
    public Texture? HandleTexture { get; set; }
    public Color4 BackgroundColor { get; set; } = Color.White;
    public Color4 HandleColor { get; set; } = Color.White;

    public float Min { get; set; } = 0f;
    public float Max { get; set; } = 100f;
    public float Step { get; set; } = 1f;

    private float _value = 0f;
    public float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, Min, Max);
    }

    public Vector2 HandleSize { get; set; } = new Vector2(8, 20);
    public Action<float>? OnValueChanged { get; set; }
    
    private static UISlider? _activeDrag = null;

    public override void Update(UIRenderer renderer)
    {
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);

        bool hovered = InputManager.MousePosition.X >= resolvedPos.X &&
                       InputManager.MousePosition.X <= resolvedPos.X + resolvedSize.X &&
                       InputManager.MousePosition.Y >= resolvedPos.Y &&
                       InputManager.MousePosition.Y <= resolvedPos.Y + resolvedSize.Y;

        if (hovered && InputManager.LeftMouseButtonDown && _activeDrag == null)
            _activeDrag = this;
        
        if (!InputManager.LeftMouseButtonDown)
            _activeDrag = null;

        if (_activeDrag == this)
        {
            float scale = resolvedSize.Y / Size.Y;
            float handleW = HandleSize.X * scale;
            float t = Math.Clamp((InputManager.MousePosition.X - resolvedPos.X - handleW / 2f) 
                                 / (resolvedSize.X - handleW), 0f, 1f);
            float raw = Min + t * (Max - Min);
            float stepped = MathF.Round(raw / Step) * Step;
            if (stepped != _value)
            {
                _value = stepped;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public override void Render(UIRenderer renderer)
    {
        if (BackgroundTexture != null)
            renderer.DrawTexturedRect(Position, Size, BackgroundTexture, BackgroundColor, Anchor);
        else
            renderer.DrawRect(Position, Size, BackgroundColor, Anchor);

        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);

        float t = Max > Min ? (_value - Min) / (Max - Min) : 0f;
        float scale = resolvedSize.Y / Size.Y;
        float handleW = HandleSize.X * scale;
        float handleH = HandleSize.Y * scale;
        float handleX = resolvedPos.X + t * (resolvedSize.X - handleW);
        float handleY = resolvedPos.Y + (resolvedSize.Y - handleH) / 2f;
        
        float s = Math.Min(renderer.ScreenSize.X / renderer.ReferenceSize.X, renderer.ScreenSize.Y / renderer.ReferenceSize.Y);
        Vector2 logicalPos = new Vector2(handleX / s, handleY / s);
        Vector2 logicalSize = new Vector2(handleW / s, handleH / s);

        if (HandleTexture != null)
            renderer.DrawTexturedRectAbsolute(new Vector2(handleX, handleY), new Vector2(handleW, handleH), HandleTexture, HandleColor);
        else
            renderer.DrawRectAbsolute(new Vector2(handleX, handleY), new Vector2(handleW, handleH), HandleColor);
    }
}