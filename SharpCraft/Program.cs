using SharpCraft.Engine;
using SharpCraft.Engine.Scene;

namespace SharpCraft;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        var window = new GameWindow();
        SceneManager.SetScene(new MainScene());
        window.Run();
    }
}

