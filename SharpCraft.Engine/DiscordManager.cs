// The entire script needs to be reworked from scratch
// Currently it's just a placeholder

using DiscordRPC;

namespace SharpCraft.Engine;

public static class DiscordManager
{
    // private static DiscordRpcClient _client;

    public static void Initialize()
    {
        // _client = new DiscordRpcClient("your_app_id");
        // _client.Initialize();
        // SetPresence($"Playing {EngineMetadata.Info.GameName}"); 
    }
    
    public static void SetPresence(string stateKey)
    {
        // _client.SetPresence(new RichPresence
        // {
        //     State = stateKey,
        //     Assets = new DiscordRPC.Assets
        //     {
        //         LargeImageKey = "logo",
        //         LargeImageText = "SharpCraft"
        //     }
        // });
    }
    
    // public static void Shutdown() => _client.Dispose();
}