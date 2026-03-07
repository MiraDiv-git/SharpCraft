namespace SharpCraft.Engine.UI;

public abstract class UIElement
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; } = new Vector2(32, 32);
    public bool Visible { get; set; } = true;
    public bool ScaleWithScreen { get; set; } = true;
    public Anchor Anchor { get; set; } = Anchor.TopLeft;

    public virtual void Update(UIRenderer renderer) { }
    public abstract void Render(UIRenderer renderer);
    
}