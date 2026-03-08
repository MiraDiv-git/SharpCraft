using Silk.NET.OpenGL;
using StbImageSharp;

namespace SharpCraft.Engine.Assets;

public static class AssetManager
{
    private static GL _gl;

    public static void Initialize(GL gl)
    {
        _gl = gl;
        Console.WriteLine("[OK] Asset manager initialized.");
    }
    
    public static (Texture texture, byte[] pixels, int width, int height) LoadFontTexture(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(0);
        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        var texture = new Texture(_gl, path);
        return (texture, image.Data, image.Width, image.Height);
    }
    
    public static Texture LoadTexture(string path, bool flipVertically = true) 
        => new Texture(_gl, path, flipVertically);
}