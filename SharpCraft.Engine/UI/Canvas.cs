using SharpCraft.Engine.Input;
using Silk.NET.Windowing;
namespace SharpCraft.Engine.UI;

public class Canvas
{
    private readonly List<UIElement> _elements = new();
    private readonly UIRenderer _renderer;

    public Canvas(UIRenderer renderer)
    {
        _renderer = renderer;
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
        {
            if (element.Visible)
                element.Render(_renderer);
        }
    }
    
    public void Clear() => _elements.Clear();
}