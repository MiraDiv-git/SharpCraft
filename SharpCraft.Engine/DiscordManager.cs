using DiscordRPC;
using SharpCraft.Engine;

namespace SharpCraft;

public static class DiscordManager
{
    private static DiscordRpcClient _client;

    public static void Initialize()
    {
        _client = new DiscordRpcClient("1480590651069304862");
        _client.Initialize();
        SetPresence("discord.main_menu"); // TODO: Add localization switch logic
    }
    
    public static void SetPresence(string stateKey)
    {
        _client.SetPresence(new RichPresence
        {
            State = Localization.Get(stateKey),
            Assets = new Assets
            {
                LargeImageKey = "logo",
                LargeImageText = "SharpCraft"
            }
        });
    }
    
    public static void Shutdown() => _client.Dispose();
}