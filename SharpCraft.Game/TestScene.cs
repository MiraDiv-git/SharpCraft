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
        LoadAsciiText();
    }

    private void LoadBackButton()
    {
        var rect = _canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 270);
        rect.Anchor = Anchor.MiddleCenter;
        rect.Size = new Vector2(350, 50);
        rect.OnClick += () =>
        {
            Console.WriteLine("[INFO] Changing scene to Main Menu Scene.");
            SceneManager.SetScene(new MainMenuScene());
        };
    }

    private void LoadAsciiText()
    {
        var line1 = _canvas.AddElement<UIText>();
        line1.Position = new Vector2(0, 0);
        line1.Anchor = Anchor.MiddleCenter;
        line1.Text = " !\"#$%&'()*+,-./";
        
        var line2 = _canvas.AddElement<UIText>();
        line2.Position = new Vector2(line1.Position.X, 20);
        line2.Anchor = Anchor.MiddleCenter;
        line2.Text = "0123456789:;<=>?";
        
        var line3 = _canvas.AddElement<UIText>();
        line3.Position = new Vector2(line2.Position.X, line2.Position.Y + 20);
        line3.Anchor = Anchor.MiddleCenter;
        line3.Text = "@ABCDEFGHIJKLMNO";
        
        var line4 = _canvas.AddElement<UIText>();
        line4.Position = new Vector2(line3.Position.X, line3.Position.Y + 20);
        line4.Anchor = Anchor.MiddleCenter;
        line4.Text = "PQRSTUVWXYZ[\\]^_";
        
        var line5 = _canvas.AddElement<UIText>();
        line5.Position = new Vector2(line4.Position.X, line4.Position.Y + 20);
        line5.Anchor = Anchor.MiddleCenter;
        line5.Text = "`abcdefghijklmno";
        
        var line6 = _canvas.AddElement<UIText>();
        line6.Position = new Vector2(line5.Position.X, line5.Position.Y + 20);
        line6.Anchor = Anchor.MiddleCenter;
        line6.Text = "pqrstuvwxyz{|}~\u0000";
    }
}