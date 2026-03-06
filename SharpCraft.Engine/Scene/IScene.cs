using SharpCraft.Engine.UI;

namespace SharpCraft.Engine.Scene;

public interface IScene
{
    void Load(UIRenderer uiRenderer);
    void Update();
    void Render();
}