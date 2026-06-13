namespace SharpCraft.Engine.UI;

public enum TextAlign { Left, Center, Right }

public class UIText : UIElement
{
    private string _text = "";
    public string Text
    {
        get => Localization.Get(_text);
        set => _text = value;
    }
    
    public string Font { get; set; }
    public float FontSize { get; set; } = Config.EngineDefaults.Font.Size;
    public Color4 FontColor { get; set; } = Config.EngineDefaults.Font.Color;
    public float Spacing { get; set; } = Config.EngineDefaults.Font.Spacing;
    public bool Shadow { get; set; } = Config.EngineDefaults.Font.Shadow;
    public float ShadowOffset { get; set; } = Config.EngineDefaults.Font.ShadowOffset;
    public float VerticalOffset { get; set; } = Config.EngineDefaults.Font.VerticalOffset;
    public TextAlign Align { get; set; } = Config.EngineDefaults.Font.TextAlign;
    
    public override void Render(UIRenderer renderer)
    {
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
    
        float screenScale = resolvedSize.Y / Size.Y;
        float scaledFontSize = FontSize * screenScale;
        float glyphScale = scaledFontSize / 8f;
        
        float totalWidth = 0;
        foreach (var c in Text)
            totalWidth += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
        
        float xOffset = Align switch {
            TextAlign.Left   => 0,
            TextAlign.Right  => resolvedSize.X - totalWidth,
            _                => (resolvedSize.X - totalWidth) / 2f
        };
        float yOffset = (resolvedSize.Y - scaledFontSize) / 2f + VerticalOffset * screenScale;
    
        if (Shadow)
        {
            float sx = xOffset;
            var shadowColor = (0f, 0f, 0f, FontColor.a * 0.5f);
            foreach (var c in Text)
            {
                renderer.DrawChar(resolvedPos + new Vector2(sx + ShadowOffset * screenScale, yOffset + ShadowOffset * screenScale), scaledFontSize, c, shadowColor);
                sx += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
            }
        }
    
        float x = xOffset;
        foreach (var c in Text)
        {
            renderer.DrawChar(resolvedPos + new Vector2(x, yOffset), scaledFontSize, c, FontColor);
            x += (renderer.GetCharWidth(c) + Spacing) * glyphScale;
        }
    }
}