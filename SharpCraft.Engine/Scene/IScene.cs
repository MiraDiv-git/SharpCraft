using SharpCraft.Engine.UI;

namespace SharpCraft.Engine.Scene;

public interface IScene
{
    void Load(UIRenderer uiRenderer);
    void Unload();
    void Update();
    void Render();
}