using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Rendering;

public class RenderContext
{
    public GL GL { get; }
    public UIRenderer UIRenderer { get; }
    
    public RenderContext(GL gl, UIRenderer uiRenderer)
    {
        GL = gl;
        UIRenderer = uiRenderer;
    }
}