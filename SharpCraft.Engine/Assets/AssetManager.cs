using System.IO.Compression;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace SharpCraft.Engine.Assets;

public static class AssetManager
{
    private static GL _gl;
    private static ZipArchive _resources;

    public static void Initialize(GL gl)
    {
        _gl = gl;
        Console.WriteLine("[OK] Asset manager initialized.");
        
        _resources = ZipFile.OpenRead("Resources.scres");
        Console.WriteLine("\t↳ Reading Resources.scres...");
    }
    
    public static Stream OpenResource(string path)
    {
        var entry = _resources.GetEntry(path.Replace('\\', '/'));
        if (entry == null) throw new FileNotFoundException($"Resource not found: {path}");
        var ms = new MemoryStream();
        using var stream = entry.Open();
        stream.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }
    
    public static (Texture texture, byte[] pixels, int width, int height) LoadFontTexture(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(0);
        var image = ImageResult.FromStream(OpenResource(path), ColorComponents.RedGreenBlueAlpha);
        var texture = new Texture(_gl, path);
        return (texture, image.Data, image.Width, image.Height);
    }
    
    public static Texture LoadTexture(string path, bool flipVertically = true) 
        => new Texture(_gl, path, flipVertically);
}