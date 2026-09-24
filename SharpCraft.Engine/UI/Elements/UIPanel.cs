using SharpCraft.Engine.Rendering;

namespace SharpCraft.Engine.UI;

public class UIPanel : UIElement
{
    public Color4 BackgroundColor { get; set; } = Color.DarkGrey.WithAlpha(200);
    public bool Border { get; set; } = true;
    public Color4 BorderColor { get; set; } = Color.Grey.WithAlpha(160);
    public float BorderThickness { get; set; } = 2f;

    public override void Render(UIRenderer renderer)
    {
        var (pos, size) = renderer.ResolveElement(Position, Size, Anchor);

        renderer.DrawRectAbsolute(pos, size, BackgroundColor);

        if (!Border || BorderThickness <= 0f) return;
        float t = BorderThickness;

        renderer.DrawRectAbsolute(pos, new Vector2(size.X, t), BorderColor);
        renderer.DrawRectAbsolute(pos + new Vector2(0, size.Y - t), new Vector2(size.X, t), BorderColor);
        renderer.DrawRectAbsolute(pos, new Vector2(t, size.Y), BorderColor);
        renderer.DrawRectAbsolute(pos + new Vector2(size.X - t, 0), new Vector2(t, size.Y), BorderColor);
    }
}