using SharpCraft.Engine.Input;
using SharpCraft.Engine.UI.Elements;

namespace SharpCraft.Engine.UI;

public class Canvas
{
    private readonly List<UIElement> _elements = new();
    private bool _active = false;

    public bool IsActive => _active;

    public Canvas()
    {
        UIRenderer.Instance!.RegisterCanvas(this);
    }

    public void SetActive(bool active)
    {
        _active = active;
    }
    
    public void SetExclusive(bool active)
    {
        if (active)
        {
            UIRenderer.Instance!.DeactivateAllCanvases();
            SetActive(true);
        }
        else
        {
            SetActive(false);
        }
    }

    public T AddElement<T>() where T : UIElement, new()
    {
        var element = new T();
        _elements.Add(element);
        return element;
    }

    public void Update(UIRenderer renderer)
    {
        InputManager.ResetCursor();
        foreach (var element in _elements.ToList())
            if (element.Visible)
                element.Update(renderer);
    }

    public void Render()
    {
        foreach (var element in _elements)
            if (element.Visible && element is not UIText)
                element.Render(UIRenderer.Instance!);

        foreach (var element in _elements)
            if (element.Visible && element is UIText)
                element.Render(UIRenderer.Instance!);
    }

    public void Clear()
    {
        _elements.Clear();
    }
}