using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Physics;

public class DebugRenderer : IDisposable
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao, _vbo;

    public DebugRenderer(GL gl, Shader shader)
    {
        _gl = gl;
        _shader = shader;
        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, 0);
        _gl.EnableVertexAttribArray(0);
    }

    public unsafe void DrawAABB(AABB aabb, Vector3 color, Matrix4X4<float> view, Matrix4X4<float> proj)
    {
        var n = aabb.Min;
        var x = aabb.Max;

        float[] v = {
            n.X,n.Y,n.Z,  x.X,n.Y,n.Z,
            x.X,n.Y,n.Z,  x.X,n.Y,x.Z,
            x.X,n.Y,x.Z,  n.X,n.Y,x.Z,
            n.X,n.Y,x.Z,  n.X,n.Y,n.Z,

            n.X,x.Y,n.Z,  x.X,x.Y,n.Z,
            x.X,x.Y,n.Z,  x.X,x.Y,x.Z,
            x.X,x.Y,x.Z,  n.X,x.Y,x.Z,
            n.X,x.Y,x.Z,  n.X,x.Y,n.Z,

            n.X,n.Y,n.Z,  n.X,x.Y,n.Z,
            x.X,n.Y,n.Z,  x.X,x.Y,n.Z,
            x.X,n.Y,x.Z,  x.X,x.Y,x.Z,
            n.X,n.Y,x.Z,  n.X,x.Y,x.Z,
        };

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, v, BufferUsageARB.DynamicDraw);

        _shader.Use();
        _shader.SetUniform("uView", view);
        _shader.SetUniform("uProjection", proj);
        _shader.SetUniform("uColor", color);

        _gl.DrawArrays(PrimitiveType.Lines, 0, 24);
    }
    
    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
    }
}