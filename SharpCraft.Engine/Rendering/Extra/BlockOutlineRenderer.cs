using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SharpCraft.Engine.Physics;

namespace SharpCraft.Engine.Rendering.Extra;

public class BlockOutlineRenderer : IDisposable
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao, _vbo;

    public BlockOutlineRenderer(GL gl, Shader shader)
    {
        _gl = gl;
        _shader = shader;
        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 28, 0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 28, 12);
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 28, 24);
        _gl.EnableVertexAttribArray(2);
        _gl.BindVertexArray(0);
    }

    public unsafe void DrawOutline(AABB aabb, Vector4D<float> color, Matrix4X4<float> view, 
        Matrix4X4<float> proj, Vector2D<float> screenSize, float thickness = 0.003f)
    {
        var n = aabb.Min;
        var x = aabb.Max;
        
        var edges = new (Vector3D<float> a, Vector3D<float> b)[]
        {
            (new(n.X,n.Y,n.Z), new(x.X,n.Y,n.Z)),
            (new(x.X,n.Y,n.Z), new(x.X,n.Y,x.Z)),
            (new(x.X,n.Y,x.Z), new(n.X,n.Y,x.Z)),
            (new(n.X,n.Y,x.Z), new(n.X,n.Y,n.Z)),
            (new(n.X,x.Y,n.Z), new(x.X,x.Y,n.Z)),
            (new(x.X,x.Y,n.Z), new(x.X,x.Y,x.Z)),
            (new(x.X,x.Y,x.Z), new(n.X,x.Y,x.Z)),
            (new(n.X,x.Y,x.Z), new(n.X,x.Y,n.Z)),
            (new(n.X,n.Y,n.Z), new(n.X,x.Y,n.Z)),
            (new(x.X,n.Y,n.Z), new(x.X,x.Y,n.Z)),
            (new(x.X,n.Y,x.Z), new(x.X,x.Y,x.Z)),
            (new(n.X,n.Y,x.Z), new(n.X,x.Y,x.Z)),
        };

        var verts = new List<float>();
        foreach (var (a, b) in edges)
        {
            void AddVert(Vector3D<float> pos, Vector3D<float> other, float side)
            {
                verts.Add(pos.X); verts.Add(pos.Y); verts.Add(pos.Z);
                verts.Add(other.X); verts.Add(other.Y); verts.Add(other.Z);
                verts.Add(side);
            }

            AddVert(a, b, -1); AddVert(a, b,  1); AddVert(b, a, -1);
            AddVert(b, a, -1); AddVert(a, b,  1); AddVert(b, a,  1);
        }

        var arr = verts.ToArray();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, arr, BufferUsageARB.DynamicDraw);

        _shader.Use();
        _shader.SetUniform("uView", view);
        _shader.SetUniform("uProjection", proj);
        _shader.SetUniform("uColor", color);
        _shader.SetUniform("uScreenSize", new Vector2(screenSize.X, screenSize.Y));
        _shader.SetUniform("uThickness", thickness);

        _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)(edges.Length * 6));
        _gl.BindVertexArray(0);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
    }
}