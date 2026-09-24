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
    
    public string Font { get; set; } = Config.EngineDefaults.Font.Path;
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
        float fontSize = FontSize * screenScale;

        var measure = renderer.MeasureText(Text, fontSize);
        float xOffset = Align switch
        {
            TextAlign.Left   => 0,
            TextAlign.Right  => resolvedSize.X - measure.X,
            _                => (resolvedSize.X - measure.X) / 2f
        };
        float yOffset = (resolvedSize.Y - measure.Y) / 2f + VerticalOffset * screenScale;

        var drawPos = resolvedPos + new Vector2(xOffset, yOffset);

        renderer.DrawText(Text, drawPos, fontSize, FontColor,
            shadow: Shadow, shadowOffset: ShadowOffset * screenScale);
    }
}