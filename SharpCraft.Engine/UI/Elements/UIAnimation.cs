using SharpCraft.Engine.Physics;

namespace SharpCraft.Engine.UI.Elements;

public class UIAnimation : UIElement
{
    public Texture? Atlas { get; set; }
    public Color4 color { get; set; } = Color.White;
    public int FrameCount { get; set; } = -1;
    public int Horizontal { get; set; } = 1;
    public int Vertical { get; set; } = 1;
    public float FrameDuration { get; set; } = 0.1f; // Frames per second
    
    private int _currentFrame = 0;
    private float _timer = 0f;
    private int TotalFrames => FrameCount > 0 ? FrameCount : Horizontal * Vertical;

    public override void Update(UIRenderer renderer)
    {
        _timer += Time.DeltaTime;
        if (_timer >= FrameDuration)
        {
            _timer -= FrameDuration;
            _currentFrame = (_currentFrame + 1) % TotalFrames;
        }
    }

    public override void Render(UIRenderer renderer)
    {
        if (Atlas == null) return;
        
        int col = _currentFrame % Horizontal;
        int row = _currentFrame / Horizontal;
        
        var uvOffset = new Vector2(col / (float)Horizontal, 1f - (row + 1) / (float)Vertical);
        var uvScale = new Vector2(1f / Horizontal, 1f / Vertical);
        
        renderer.DrawTexturedRectUV(Position, Size, Atlas, color, Anchor, uvOffset, uvScale);
    }
}