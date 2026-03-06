using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Game;

public class MainMenuScene : IScene
{
    private UIRenderer _uiRenderer;
    private Canvas _canvas;

    public void Load(UIRenderer uiRenderer)
    {
        _uiRenderer = uiRenderer;
        _canvas = new Canvas(_uiRenderer);
        LoadMainMenu();
    }

    public void Update() => _canvas.Update(_uiRenderer);

    public void Render() => _canvas.Render();

    private void LoadMainMenu()
    {
        PlayButton();
        OptionsButton();
        ExitButton();
    }
    
    private void PlayButton()
    {
        var rect = _canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 0);
        rect.Size = new Vector2(350, 50);
        rect.Anchor = Anchor.MiddleCenter;
        rect.ButtonColor = Color.Grey;
        rect.HoverColor = Color.LightGrey;
        rect.PressColor = Color.White;
        rect.OnClick += () => Console.WriteLine("[INFO] Play button clicked.");
    }
    
    private void OptionsButton()
    {
        var rect = _canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 100);
        rect.Size = new Vector2(350, 50);
        rect.Anchor = Anchor.MiddleCenter;
        rect.ButtonColor = Color.Grey;
        rect.HoverColor = Color.LightGrey;
        rect.PressColor = Color.White;
        rect.OnClick += () => Console.WriteLine("[INFO] Options button clicked.");
    }
    
    private void ExitButton()
    {
        var rect = _canvas.AddElement<UIButton>();
        rect.Position = new Vector2(0, 200);
        rect.Size = new Vector2(350, 50);
        rect.Anchor = Anchor.MiddleCenter;
        rect.ButtonColor = Color.Grey;
        rect.HoverColor = Color.LightGrey;
        rect.PressColor = Color.White;
        rect.OnClick += () => Environment.Exit(0);
    }
}