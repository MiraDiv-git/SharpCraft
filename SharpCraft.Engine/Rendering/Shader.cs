using Silk.NET.OpenGL;
using Silk.NET.Maths;
namespace SharpCraft.Engine.Rendering;

public class Shader
{
    private readonly uint _handle;
    private readonly GL _gl;

    public Shader(GL gl, string vertPath, string fragPath)
    {
        _gl = gl;

        var vert = Compile(ShaderType.VertexShader, File.ReadAllText(vertPath));
        var frag = Compile(ShaderType.FragmentShader, File.ReadAllText(fragPath));

        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vert);
        _gl.AttachShader(_handle, frag);
        _gl.LinkProgram(_handle);
        
        _gl.GetProgram(_handle, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Error linking shader program: {_gl.GetProgramInfoLog(_handle)}");
        }

        _gl.DeleteShader(vert);
        _gl.DeleteShader(frag);
    }

    private uint Compile(ShaderType type, string source)
    {
        var shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);
        
        string infoLog = _gl.GetShaderInfoLog(shader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}: {infoLog}");
        }
        
        return shader;
    }
    
    // Color
    public void SetUniform(string name, Color4 color)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location != -1)
            _gl.Uniform4(location, color.r, color.g, color.b, color.a);
    }
    
    // Position and size
    public void SetUniform(string name, Vector2 value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location != -1)
            _gl.Uniform2(location, value.X, value.Y);
    }
    
    // Camera (3D)
    public unsafe void SetUniform(string name, Matrix4X4<float> matrix)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location != -1)
            _gl.UniformMatrix4(location, 1, false, (float*)&matrix);
    }
    
    // Only number (used for texture)
    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        if (location != -1)
            _gl.Uniform1(location, value);
    }

    public void Use() => _gl.UseProgram(_handle);
}