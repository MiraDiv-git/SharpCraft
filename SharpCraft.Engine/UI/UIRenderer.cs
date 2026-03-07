using Silk.NET.OpenGL;

namespace SharpCraft.Engine.UI;

public class UIRenderer
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly Vector2 _referenceSize;
    
    private Vector2 _screenSize;
    
    public unsafe UIRenderer(GL gl, int width, int height)
    {
        _gl = gl;
        _referenceSize = new Vector2(width, height);
        _screenSize = new Vector2(width, height);
        
        _shader = new Shader(gl, "Shaders/UI/ui.vert", "Shaders/UI/ui.frag");
        
        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();
        
        float[] vertices = {
            0f, 0f,     0f, 1f,
            1f, 0f,     1f, 1f,
            0f, 1f,     0f, 0f,
            1f, 1f,     1f, 0f,
        };
        
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        fixed (float* v = vertices)
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), 
                v, BufferUsageARB.StaticDraw);
        
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 
            4 * sizeof(float), (void*)0); // Position
        _gl.EnableVertexAttribArray(0);
        
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 
            4 * sizeof(float), (void*)(2 * sizeof(float))); // UV
        _gl.EnableVertexAttribArray(1);
        
        _gl.BindVertexArray(0);
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }
    
    public void SetScreenSize(int width, int height)
    {
        _screenSize = new Vector2(width, height);
    }

    private Vector2 ResolvePosition(Vector2 position, Vector2 size, Anchor anchor)
    {
        float scaleX = _screenSize.X / _referenceSize.X;
        float scaleY = _screenSize.Y / _referenceSize.Y;
        float px = position.X * scaleX;
        float py = position.Y * scaleY;

        return anchor switch
        {
            Anchor.TopLeft      => new Vector2(px, py),
            Anchor.TopCenter    => new Vector2(_screenSize.X / 2 - size.X / 2 + px, py),
            Anchor.TopRight     => new Vector2(_screenSize.X - size.X + px, py),
            Anchor.MiddleLeft   => new Vector2(px, _screenSize.Y / 2 - size.Y / 2 + py),
            Anchor.MiddleCenter => new Vector2(_screenSize.X / 2 - size.X / 2 + px, _screenSize.Y / 2 - size.Y / 2 + py),
            Anchor.MiddleRight  => new Vector2(_screenSize.X - size.X + px, _screenSize.Y / 2 - size.Y / 2 + py),
            Anchor.BottomLeft   => new Vector2(px, _screenSize.Y - size.Y + py),
            Anchor.BottomCenter => new Vector2(_screenSize.X / 2 - size.X / 2 + px, _screenSize.Y - size.Y + py),
            Anchor.BottomRight  => new Vector2(_screenSize.X - size.X + px, _screenSize.Y - size.Y + py),
            _ => new Vector2(px, py)
        };
    }
    
    private Vector2 ResolveSize(Vector2 size)
    {
        float scale = Math.Min(_screenSize.X / _referenceSize.X, _screenSize.Y / _referenceSize.Y);
        return new Vector2(size.X * scale, size.Y * scale);
    }
    
    public (Vector2 pos, Vector2 size) ResolveElement(Vector2 position, Vector2 size, Anchor anchor)
    {
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        return (resolvedPos, resolvedSize);
    }

    public void DrawRect(Vector2 position, Vector2 size, Color4 color, Anchor anchor)
    {
        _shader.Use();
        
        _shader.SetUniform("uColor", color);
        
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        _shader.SetUniform("uOffset", resolvedPos);
        _shader.SetUniform("uScale", resolvedSize);
        _shader.SetUniform("uUseTexture", 0);
        
        _shader.SetUniform("uScreenSize", _screenSize);

        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        _gl.BindVertexArray(0);
    }

    public void DrawTexturedRect(Vector2 position, Vector2 size, Texture texture, Color4 color, Anchor anchor)
    {
        _shader.Use();
        texture.Bind();
        
        _shader.SetUniform("uTexture", 0);
        _shader.SetUniform("uUseTexture", 1);
        _shader.SetUniform("uColor", color);
        
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        _shader.SetUniform("uOffset", resolvedPos);
        _shader.SetUniform("uScale", resolvedSize);
        
        _shader.SetUniform("uScreenSize", _screenSize);
        
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        _gl.BindVertexArray(0);
    }
}