using SharpCraft.Engine.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.World.Blocks;

public class Block : IDisposable
{
    private readonly Mesh _mesh;
    private readonly Shader _shader;
    private readonly Texture? _texture;

    public Color4 Color { get; } = (0.8f, 0f, 0.8f, 1f);

    public Block(GL gl, Shader shader, Texture? texture = null, float[]? vertices = null)
    {
        _shader = shader;
        _texture = texture;
        _mesh = new Mesh(gl, vertices ?? BlockMeshes.Cube, hasUV: true);
    }

    public void Draw(Matrix4X4<float> model)
    {
        _shader.Use();
        _shader.SetUniform("uModel", model);
        _shader.SetUniform("uUseTexture", _texture != null ? 1 : 0);

        if (_texture != null)
            _texture.Bind();
        else
            _shader.SetUniform("uColor", Color);

        _mesh.Draw();
    }

    // Textures are owned and reused by AssetManager, only the mesh belongs here.
    public void Dispose() => _mesh.Dispose();
}