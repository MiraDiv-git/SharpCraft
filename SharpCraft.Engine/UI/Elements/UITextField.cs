using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using Silk.NET.Input;

namespace SharpCraft.Engine.UI;

public class UITextField : UIElement
{
    public string Text { get; set; } = "";
    public Color4 BackgroundColor { get; set; } = Color.Black.WithAlpha(180);
    public Color4 TextColor { get; set; } = Color.White;
    public float FontSize { get; set; } = 9f;
    public int MaxLength { get; set; } = 256;
    public Action<string>? OnSubmit { get; set; }
    public Action? OnCancel { get; set; }

    private float _cursorTimer = 0f;
    private bool _cursorVisible = true;
    private bool _isFocused = false;
    private float _backspaceTimer = 0f;

    public override void Update(UIRenderer renderer)
    {
        if (!IsFocused) return;

        _cursorTimer += Time.DeltaTime;
        if (_cursorTimer >= 0.5f) { _cursorVisible = !_cursorVisible; _cursorTimer = 0f; }

        if (InputManager.IsKeyJustPressed(Key.Enter))
        {
            OnSubmit?.Invoke(Text);
            Text = "";
            IsFocused = false;
            return;
        }

        if (InputManager.IsKeyJustPressed(Key.Escape))
        {
            Text = "";
            IsFocused = false;
            OnCancel?.Invoke();
            return;
        }

        if (InputManager.IsKeyDown(Key.Backspace))
        {
            if (Text.Length > 0)
            {
                if (InputManager.IsKeyJustPressed(Key.Backspace))
                {
                    Text = Text[..^1];
                    _backspaceTimer = 0.5f;
                }
                else
                {
                    _backspaceTimer -= Time.DeltaTime;
                    if (_backspaceTimer <= 0)
                    {
                        Text = Text[..^1];
                        _backspaceTimer = 0.04f;
                    }
                }
            }
            else
                _backspaceTimer = 0;
        }
    }

    public override void Render(UIRenderer renderer)
    {
        renderer.DrawRect(Position, Size, BackgroundColor, Anchor);
        
        var display = IsFocused && _cursorVisible ? Text + "|" : Text;
        
        var (resolvedPos, resolvedSize) = renderer.ResolveElement(Position, Size, Anchor);
        float scale = resolvedSize.Y / Size.Y;
        float x = resolvedPos.X + 4 * scale;
        float y = resolvedPos.Y + (resolvedSize.Y - FontSize * scale) / 2f;

        foreach (var c in display)
        {
            renderer.DrawChar(new Vector2(x, y), FontSize * scale, c, TextColor);
            x += (renderer.GetCharWidth(c) + 1) * scale;
        }
    }
    
    public bool IsFocused
    {
        get => _isFocused;
        set
        {
            if (_isFocused == value) return;
            _isFocused = value;
            if (_isFocused)
                InputManager.OnKeyChar += HandleChar;
            else
                InputManager.OnKeyChar -= HandleChar;
        }
    }

    private void HandleChar(char c)
    {
        if (Text.Length < MaxLength)
            Text += c;
    }
}