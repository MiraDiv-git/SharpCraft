using Silk.NET.Maths;
using Silk.NET.OpenGL;
namespace SharpCraft.Engine.Rendering;

public class Mesh : IDisposable
{
    private readonly GL _gl;
    private readonly uint _vao, _vbo;
    private uint _ebo;
    private readonly int _vertexCount;
    private readonly int _indexCount;
    private readonly int _stride;
    private bool _hasIndices;

    private uint _instanceVbo;
    private int _instanceCount;

    public unsafe Mesh(GL gl, float[] vertices, bool hasUV = false, bool hasColor = false)
    {
        _gl = gl;
        _stride = hasUV ? 5 : hasColor ? 6 : 3;
        _vertexCount = vertices.Length / _stride;

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        fixed (float* v = vertices)
            _gl.BufferData(BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                v, BufferUsageARB.StaticDraw);

        SetupVertexAttributes(hasUV, hasColor);

        _gl.BindVertexArray(0);
    }

    public unsafe Mesh(GL gl, float[] vertices, uint[] indices, bool hasUV = false, bool hasColor = false)
        : this(gl, vertices, hasUV, hasColor)
    {
        _hasIndices = true;
        _indexCount = indices.Length;

        _gl.BindVertexArray(_vao);
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

        fixed (uint* i = indices)
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                i, BufferUsageARB.StaticDraw);

        _gl.BindVertexArray(0);
    }

    private unsafe void SetupVertexAttributes(bool hasUV, bool hasColor)
    {
        uint stride = (uint)(_stride * sizeof(float));
        if (hasUV)
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
            _gl.EnableVertexAttribArray(1);
        }
        else if (hasColor)
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
            _gl.EnableVertexAttribArray(1);
        }
        else
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
            _gl.EnableVertexAttribArray(0);
        }
    }

    public unsafe void SetInstances(Matrix4X4<float>[] models)
    {
        if (_instanceVbo == 0)
            _instanceVbo = _gl.GenBuffer();

        _instanceCount = models.Length;
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _instanceVbo);

        fixed (Matrix4X4<float>* m = models)
            _gl.BufferData(BufferTargetARB.ArrayBuffer,
                (nuint)(models.Length * sizeof(Matrix4X4<float>)),
                m, BufferUsageARB.DynamicDraw);

        for (uint i = 0; i < 4; i++)
        {
            _gl.VertexAttribPointer(2 + i, 4, VertexAttribPointerType.Float, false,
                (uint)sizeof(Matrix4X4<float>), (void*)(i * 16));
            _gl.EnableVertexAttribArray(2 + i);
            _gl.VertexAttribDivisor(2 + i, 1);
        }

        _gl.BindVertexArray(0);
    }

    public void Draw()
    {
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertexCount);
        _gl.BindVertexArray(0);
    }

    public unsafe void DrawIndexed()
    {
        _gl.BindVertexArray(_vao);
        _gl.DrawElements(PrimitiveType.Triangles, (uint)_indexCount, DrawElementsType.UnsignedInt, (void*)0);
        _gl.BindVertexArray(0);
    }

    public void DrawInstanced()
    {
        if (_instanceCount == 0) return;
        _gl.BindVertexArray(_vao);
        _gl.DrawArraysInstanced(PrimitiveType.Triangles, 0, (uint)_vertexCount, (uint)_instanceCount);
        _gl.BindVertexArray(0);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        if (_instanceVbo != 0) _gl.DeleteBuffer(_instanceVbo);
        if (_hasIndices) _gl.DeleteBuffer(_ebo);
    }
}