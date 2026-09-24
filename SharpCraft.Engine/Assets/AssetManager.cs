using System.IO.Compression;
using System.Linq;
using SharpCraft.Engine.Rendering;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Assets;

public static class AssetManager
{
    private static GL _gl = null!;
    private static ZipArchive _resources = null!;
    private static readonly Dictionary<string, Texture> _textureCache = new(StringComparer.OrdinalIgnoreCase);

    public static void Initialize(GL gl)
    {
        if (!File.Exists("Resources.scres"))
            throw new FileNotFoundException("Resources.scres not found. The resources bundle must be built next to the executable.", "Resources.scres");

        _gl = gl;
        _resources = ZipFile.OpenRead("Resources.scres");
        Console.WriteLine("[LOAD] Asset manager initialized.");
    }

    public static Stream OpenResource(string path)
    {
        var entry = _resources.GetEntry(path.Replace('\\', '/'))
                    ?? throw new FileNotFoundException($"Resource not found: {path}");
        var ms = new MemoryStream();
        using var stream = entry.Open();
        stream.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }

    public static byte[] ReadResourceBytes(string path)
    {
        using var stream = OpenResource(path);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static Texture LoadTexture(string path, bool flipVertically = true)
    {
        if (_textureCache.TryGetValue(path, out var cached))
            return cached;

        var texture = new Texture(_gl, path, flipVertically, generateMipmaps: false);
        _textureCache[path] = texture;
        return texture;
    }

    public static void Dispose()
    {
        foreach (var texture in _textureCache.Values)
            texture.Dispose();
        _textureCache.Clear();

        _resources?.Dispose();
        _resources = null!;
    }
}