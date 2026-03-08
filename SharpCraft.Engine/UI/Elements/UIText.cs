namespace SharpCraft.Engine.UI.Elements;

public class UIText : UIElement
{
    public string Text { get; set; } = "";
    public float FontSize { get; set; } = 16f;
    public Color4 TextColor { get; set; } = Color.White;
    
    public float Spacing { get; set; } = 0.6f;
    public bool Shadow { get; set; } = true;
    public float ShadowOffset { get; set; } = 2f;
    public float VerticalOffset { get; set; } = 0f;
    
    public override void Render(UIRenderer renderer)
    {
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
    
        float screenScale = resolvedSize.Y / Size.Y;
        float scaledFontSize = FontSize * screenScale;
        float glyphScale = scaledFontSize / 8f;
        
        float totalWidth = 0;
        foreach (var c in Text)
            totalWidth += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
        
        float xOffset = (resolvedSize.X - totalWidth) / 2f;
        float yOffset = (resolvedSize.Y - scaledFontSize) / 2f + VerticalOffset * screenScale;
    
        if (Shadow)
        {
            float sx = xOffset;
            var shadowColor = (0f, 0f, 0f, TextColor.a * 0.5f);
            foreach (var c in Text)
            {
                renderer.DrawChar(resolvedPos + new Vector2(sx + ShadowOffset * screenScale, yOffset + ShadowOffset * screenScale), scaledFontSize, c, shadowColor);
                sx += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
            }
        }
    
        float x = xOffset;
        foreach (var c in Text)
        {
            renderer.DrawChar(resolvedPos + new Vector2(x, yOffset), scaledFontSize, c, TextColor);
            x += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
        }
    }
}