using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game;

public class TestScene : IScene
{
    private UIRenderer _uiRenderer;
    private Canvas _canvas;
    
    public void Render() => _canvas.Render();

    public void Update() => _canvas.Update(_uiRenderer);

    public void Load(UIRenderer uiRenderer)
    {
        Console.WriteLine("[INFO] Loading test scene.");
        _uiRenderer = uiRenderer;
        _canvas = new Canvas(_uiRenderer);
        LoadThisThing();
    }

    public void Unload() => _canvas.Clear();

    private void LoadThisThing()
    {
        LoadBackButton();
    }

    private void LoadBackButton()
    {
        var rect = _canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 0);
        rect.Anchor = Anchor.MiddleCenter;
        rect.Size = new Vector2(350, 50);
        rect.OnClick += () =>
        {
            Console.WriteLine("[INFO] Changing scene to Main Menu Scene.");
            SceneManager.SetScene(new MainMenuScene());
        };
    }
}