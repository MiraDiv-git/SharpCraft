using SharpCraft.Engine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.World.Blocks;

public class Block //: IDisposable
{
    private readonly Mesh _mesh;
    private readonly Shader _shader;
    private readonly Texture? _texture;

    public Block(GL gl, Shader shader, Texture? texture = null)
    {
        _shader = shader;
        _texture = texture;
        _mesh = new Mesh(gl, BlockMeshes.Cube, hasUV: true);
    }
    
    public Block(GL gl, Shader shader, Texture? texture, float[] vertices)
    {
        _shader = shader;
        _texture = texture;
        _mesh = new Mesh(gl, vertices, hasUV: true);
    }

    public void Draw(Matrix4X4<float> model)
    {
        _shader.Use();
        _shader.SetUniform("uModel", model);
        _shader.SetUniform("uUseTexture", _texture != null ? 1 : 0);
        
        if (_texture != null)
            _texture.Bind();
        else
            _shader.SetUniform("uColor", (0.8f, 0f, 0.8f, 1f));
        
        _mesh.Draw();
    }

    public void Dispose(GL _gl)
    {
        
    }
}