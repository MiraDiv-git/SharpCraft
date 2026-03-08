using SharpCraft.Engine.Assets;
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
        LoadLoadingAnimation();
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

    private void LoadLoadingAnimation()
    {
        var anim = _canvas.AddElement<UIAnimation>();
        anim.Atlas = AssetManager.LoadTexture("Textures/Animations/test_squares.png");
        anim.Position = new Vector2(200, 0);
        anim.Size = new Vector2(64, 64);
        anim.Anchor = Anchor.MiddleLeft;
        anim.Horizontal = 4;
        anim.Vertical = 2;
        anim.FrameDuration = 0.5f;
    }

    private void LoadAsciiText()
    {
        var line1 = _canvas.AddElement<UIText>();
        line1.Position = new Vector2(0, -50);
        line1.Anchor = Anchor.MiddleCenter;
        line1.Text = " !\"#$%&'()*+,-./";
        
        var line2 = _canvas.AddElement<UIText>();
        line2.Position = new Vector2(line1.Position.X, line1.Position.Y + 20);
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
        
        var line7 = _canvas.AddElement<UIText>();
        line7.Position = new Vector2(line6.Position.X, line6.Position.Y + 20);
        line7.Anchor = Anchor.MiddleCenter;
        line7.Text = "АБВГДЕЖЗИЙКЛМНОП";
        
        var line8 = _canvas.AddElement<UIText>();
        line8.Position = new Vector2(line7.Position.X, line7.Position.Y + 20);
        line8.Anchor = Anchor.MiddleCenter;
        line8.Text = "РСТУФХЦЧШЩЪЫЬЭЮЯ";
        
        var line9 = _canvas.AddElement<UIText>();
        line9.Position = new Vector2(line8.Position.X, line8.Position.Y + 20);
        line9.Anchor = Anchor.MiddleCenter;
        line9.Text = "абвгдежзиклмноп";
        
        var line10 = _canvas.AddElement<UIText>();
        line10.Position = new Vector2(line9.Position.X, line9.Position.Y + 20);
        line10.Anchor = Anchor.MiddleCenter;
        line10.Text = "рстуфхцчшщъыьэюя";
        
        var line11 = _canvas.AddElement<UIText>();
        line11.Position = new Vector2(line10.Position.X, line10.Position.Y + 20);
        line11.Anchor = Anchor.MiddleCenter;
        line11.Text = "ЁёЄЇІєїі";
    }
}