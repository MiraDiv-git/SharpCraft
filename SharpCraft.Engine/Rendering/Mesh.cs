using Silk.NET.OpenGL;
namespace SharpCraft.Engine.Rendering;

public class Mesh : IDisposable
{
    private readonly GL _gl;
    private readonly uint _vao, _vbo;
    private readonly int _vertexCount;

    public unsafe Mesh(GL gl, float[] vertices, bool hasUV = false)
    {
        _gl = gl;
        _vertexCount = vertices.Length / (hasUV ? 5 : 3);

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        fixed (float* v = vertices)
            _gl.BufferData(BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                v, BufferUsageARB.StaticDraw);

        if (hasUV)
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            _gl.EnableVertexAttribArray(1);
        }
        else
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
            _gl.EnableVertexAttribArray(0);
        }
    
        _gl.BindVertexArray(0);
    }

    public void Draw()
    {
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertexCount);
        _gl.BindVertexArray(0);
    }
    
    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
    }
}