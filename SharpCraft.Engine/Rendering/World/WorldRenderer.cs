using SharpCraft.Engine.Physics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Rendering.World;

public sealed class WorldRenderer : IDisposable
{
    private readonly GL _gl;
    private readonly Dictionary<Vector3D<int>, ChunkMesh> _chunks = new();

    public WorldRenderer(GL gl)
    {
        _gl = gl;
    }

    public int ChunkCount => _chunks.Count;

    public void SetChunk(Vector3D<int> chunkCoord, Chunk chunk, Func<byte, (Vector3 color, bool solid)> palette)
    {
        if (_chunks.TryGetValue(chunkCoord, out var old))
        {
            old.Dispose();
            _chunks.Remove(chunkCoord);
        }

        _chunks[chunkCoord] = ChunkMeshBuilder.Build(_gl, chunk, palette);
    }

    public bool RemoveChunk(Vector3D<int> chunkCoord)
    {
        if (_chunks.Remove(chunkCoord, out var mesh))
        {
            mesh.Dispose();
            return true;
        }
        return false;
    }

    public void Clear()
    {
        foreach (var mesh in _chunks.Values)
            mesh.Dispose();
        _chunks.Clear();
    }

    public void Draw(Shader shader, Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        if (_chunks.Count == 0) return;

        shader.Use();
        shader.SetUniform("uView", view);
        shader.SetUniform("uProjection", projection);

        foreach (var (coord, mesh) in _chunks)
        {
            shader.SetUniform("uModel",
                Matrix4X4.CreateTranslation(
                    (float)(coord.X * Chunk.Width),
                    (float)(coord.Y * Chunk.Height),
                    (float)(coord.Z * Chunk.Depth)));
            mesh.Draw();
        }
    }

    public void Dispose() => Clear();
}