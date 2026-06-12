using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Scene;

public interface IScene
{
    // Required
    void Load();
    void Render();
    
    // Optional
    void Unload() { }
    void Update() { }
}