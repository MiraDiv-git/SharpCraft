using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Assets;

public static class AssetManager
{
    private static GL _gl;

    public static void Initialize(GL gl)
    {
        _gl = gl;
        Console.WriteLine("[OK] Asset manager initialized.");
    }
    
    public static Texture LoadTexture(string path) => new Texture(_gl, path);
}