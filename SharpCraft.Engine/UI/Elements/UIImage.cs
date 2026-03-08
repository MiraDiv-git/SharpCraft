namespace SharpCraft.Engine.UI.Elements;

public class UIImage : UIElement
{
    public Texture? ImageTexture { get; set; }
    public Color4 ImageColor { get; set; } = Color.White;
    public override void Render(UIRenderer renderer)
    {
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
        if (ImageTexture != null)
            renderer.DrawTexturedRect(Position, Size, ImageTexture, ImageColor, Anchor);
        else
            renderer.DrawRect(Position, Size, ImageColor, Anchor);
    }
}