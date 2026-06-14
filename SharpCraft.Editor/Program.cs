using SharpCraft.Engine;
using SharpCraft.Engine.Config;
using SharpCraft.Engine.Scene;

namespace SharpCraft.Editor;

class Program
{
    static void Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        OverrideConfigs();
        var window = new GameWindow();
        SceneManager.SetScene(new EditorScene());
        window.Run();
    }

    static void OverrideConfigs()
    {
        EngineDefaults.Window.Title = "SharpCraft Editor";
        EngineMetadata.Info.GameName = "SharpCraft Editor";
        EngineMetadata.Info.GameAuthor = "MiraDiv";
        EngineMetadata.Info.GameWebsite = "https://github.com/MiraDiv-git/SharpCraft";
    }
}