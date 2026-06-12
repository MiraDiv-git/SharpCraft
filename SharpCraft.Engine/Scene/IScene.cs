using SharpCraft.Engine.UI;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Scene;

public interface IScene
{
    // Required
    void Load();
    
    // Optional
    void Render() { }
    void Unload() { }
    void Update() { }
}