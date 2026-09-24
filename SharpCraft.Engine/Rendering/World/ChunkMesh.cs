using SharpCraft.Engine.Rendering;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Rendering.World;

public sealed class ChunkMesh : IDisposable
{
    private readonly Mesh _mesh;

    internal ChunkMesh(Mesh mesh) => _mesh = mesh;

    public void Draw() => _mesh.DrawIndexed();

    public void Dispose() => _mesh.Dispose();
}