using SharpCraft.Engine.Assets;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace SharpCraft.Engine.Rendering;

public class Texture : IDisposable
{
    private uint _handle;
    private readonly GL _gl;

    internal uint Handle => _handle;

    public Texture(GL gl, string path, bool flipVertically = true, bool generateMipmaps = false)
    {
        _gl = gl;

        using var stream = AssetManager.OpenResource(path);
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        _handle = CreateTexture(image.Data, image.Width, image.Height, generateMipmaps);
    }

    public Texture(GL gl, byte[] pixels, int width, int height, bool generateMipmaps = false)
    {
        _gl = gl;
        _handle = CreateTexture(pixels, width, height, generateMipmaps);
    }

    private Texture(GL gl)
    {
        _gl = gl;
    }

    /// <summary>
    /// Allocates a texture with no initial data. Use <see cref="UploadSubData"/> to
    /// fill regions later (used by the runtime font atlas).
    /// </summary>
    public static Texture Empty(GL gl, int width, int height)
    {
        var texture = new Texture(gl);
        var handle = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, handle);

        unsafe
        {
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                (uint)width, (uint)height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, null);
        }

        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        gl.BindTexture(TextureTarget.Texture2D, 0);
        texture._handle = handle;
        return texture;
    }

    public void UploadSubData(int x, int y, int width, int height, byte[] rgba)
    {
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
        unsafe
        {
            fixed (byte* ptr = rgba)
                _gl.TexSubImage2D((GLEnum)TextureTarget.Texture2D, 0, x, y,
                    (uint)width, (uint)height, (GLEnum)PixelFormat.Rgba,
                    (GLEnum)PixelType.UnsignedByte, ptr);
        }
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    private uint CreateTexture(byte[] pixels, int width, int height, bool generateMipmaps)
    {
        var handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, handle);

        unsafe
        {
            fixed (byte* ptr = pixels)
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                    (uint)width, (uint)height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        if (generateMipmaps)
        {
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        else
        {
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        _gl.BindTexture(TextureTarget.Texture2D, 0);
        return handle;
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose() => _gl.DeleteTexture(_handle);
}