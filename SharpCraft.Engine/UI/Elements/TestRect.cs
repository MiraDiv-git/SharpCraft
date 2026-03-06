using SharpCraft.Engine.Rendering;

namespace SharpCraft.Engine.UI.Elements;

public class TestRect : UIElement
{
    public override void Render(UIRenderer renderer)
    {
        renderer.DrawRect(Position, Size, Color.Crimson, Anchor);
    }
}