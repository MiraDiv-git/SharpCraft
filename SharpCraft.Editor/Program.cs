using SharpCraft.Engine;
using SharpCraft.Engine.Scene;

namespace SharpCraft.Editor;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        var window = new GameWindow("SharpCraft Editor", 1280, 720);
        SceneManager.SetScene(new EditorScene());
        window.Run();
    }
}