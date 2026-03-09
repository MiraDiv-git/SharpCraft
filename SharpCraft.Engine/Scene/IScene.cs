using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Scene;

public interface IScene
{
    void Load(UIRenderer uiRenderer) { }
    void Load(UIRenderer uiRenderer, GL gl) => Load(uiRenderer);
    void Unload();
    void Update() { }
    void Render();
}