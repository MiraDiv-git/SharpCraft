using Silk.NET.OpenGL;
namespace SharpCraft.Engine.Rendering;

public class Mesh
{
    private readonly GL _gl;
    private readonly uint _vao, _vbo;
    private readonly int _vertexCount;

    public Mesh(GL gl, float[] vertices)
    {
        _gl = gl;
        _vertexCount = vertices.Length / 3;

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (float* v = vertices)
                _gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint)(vertices.Length * sizeof(float)),
                    v, BufferUsageARB.StaticDraw);
        }

        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        _gl.EnableVertexAttribArray(0);
    }

    public void Draw() => _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertexCount);
}