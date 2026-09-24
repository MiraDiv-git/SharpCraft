using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;

namespace SharpCraft;

public class MainScene : IScene
{
    private Canvas? _canvas;

    public void Load()
    {
        _canvas = new Canvas();
        _canvas.SetActive(true);

        var title = _canvas.AddElement<UIText>();
        title.Anchor = Anchor.MiddleCenter;
        title.Position = new Vector2(0, -40);
        title.Size = new Vector2(640, 56);
        title.Text = "Welcome to SharpCraft!";
        title.FontSize = 48f;
        title.FontColor = (0.95f, 0.95f, 1f, 1f);

        var hint = _canvas.AddElement<UIText>();
        hint.Anchor = Anchor.MiddleCenter;
        hint.Position = new Vector2(0, 24);
        hint.Size = new Vector2(640, 24);
        hint.Text = "Engine runtime is running. Build your game scenes here.";
        hint.FontSize = 18f;
        hint.FontColor = (0.65f, 0.65f, 0.8f, 1f);
    }

    public void Unload()
    {
        _canvas?.Dispose();
        _canvas = null;
    }
}