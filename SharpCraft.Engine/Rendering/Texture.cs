using Silk.NET.OpenGL;
using StbImageSharp;

namespace SharpCraft.Engine.Rendering;

public class Texture : IDisposable
{
    private readonly uint _handle;
    private readonly GL _gl;

    public Texture(GL gl, string path)
    {
        _gl = gl;
        
        StbImage.stbi_set_flip_vertically_on_load(1);
        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        
        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
        
        unsafe
        {
            fixed (byte* ptr = image.Data)
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                    (uint)image.Width, (uint)image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }
        
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public void Dispose() => _gl.DeleteTexture(_handle);
}