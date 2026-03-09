using SharpCraft.Engine;
using SharpCraft.Engine.Scene;
using SharpCraft.Game;

namespace SharpCraft;

class Program
{
    static void Main()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        var window = new GameWindow();
        SceneManager.SetScene(new MainMenuScene());
        window.Run();
    }
}

