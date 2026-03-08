using Silk.NET.OpenGL;

namespace SharpCraft.Engine.UI;

public class UIRenderer
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly Vector2 _referenceSize;
    private Texture _fontTexture;
    private Vector2 _screenSize;
    private float[] _charWidths = new float[256];
    
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
    
    public void SetFont(Texture fontAtlas, byte[] rawPixels, int atlasWidth, int atlasHeight)
    {
        _fontTexture = fontAtlas;
    
        int cellW = atlasWidth / 16;
        int cellH = atlasHeight / 16;
    
        for (int i = 0; i < 256; i++)
        {
            int col = i % 16;
            int row = i / 16;
        
            int lastFilledCol = 0;
            for (int x = 0; x < cellW; x++)
            {
                for (int y = 0; y < cellH; y++)
                {
                    int px = col * cellW + x;
                    int py = row * cellH + y;
                    int idx = (py * atlasWidth + px) * 4;
                    if (rawPixels[idx + 3] > 0)
                        lastFilledCol = x;
                }
            }
            _charWidths[i] = lastFilledCol + 1;
        }
    }
    
    public void SetScreenSize(int width, int height)
    {
        _screenSize = new Vector2(width, height);
    }

    private Vector2 ResolvePosition(Vector2 position, Vector2 size, Anchor anchor)
    {
        float scale = Math.Min(_screenSize.X / _referenceSize.X, _screenSize.Y / _referenceSize.Y);
        float px = position.X * scale;
        float py = position.Y * scale;

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
        _shader.SetUniform("uUVOffset", new Vector2(0f, 0f));
        _shader.SetUniform("uUVScale", new Vector2(1f, 1f));
        
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
        _shader.SetUniform("uUVOffset", new Vector2(0f, 0f));
        _shader.SetUniform("uUVScale", new Vector2(1f, 1f));
        
        _shader.SetUniform("uScreenSize", _screenSize);
        
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        _gl.BindVertexArray(0);
    }

    public float GetCharWidth(char c)
    {
        int index = (int)c;
        if (index >= 256) index = '?';
        return _charWidths[index];
    } 
    
    public void DrawChar(Vector2 pixelPos, float size, char character, Color4 color)
    {
        int index = (int)character;
        if (index >= 0x0410 && index < 0x0450)
            index = index - 0x0410 + 128; // Cyrillic
        else if (index == 0x0401) index = 192; // Ё
        else if (index == 0x0451) index = 193; // ё
        else if (index == 0x0404) index = 194; // Є
        else if (index == 0x0407) index = 195; // Ї
        else if (index == 0x0406) index = 196; // І
        else if (index == 0x0454) index = 197; // є
        else if (index == 0x0457) index = 198; // ї
        else if (index == 0x0456) index = 199; // і
        else if (index >= 256) index = '?'; // Unknow symbol (fallback)
        
        int col = index % 16;
        int row = 15 - (index / 16);

        var uvOffset = new Vector2(col / 16f, row / 16f);
        var uvScale = new Vector2(1f / 16f, 1f / 16f);
    
        _shader.Use();
        _fontTexture.Bind();
    
        _shader.SetUniform("uTexture", 0);
        _shader.SetUniform("uUseTexture", 1);
        _shader.SetUniform("uColor", color);
        _shader.SetUniform("uUVOffset", uvOffset);
        _shader.SetUniform("uUVScale", uvScale);
    
        _shader.SetUniform("uOffset", pixelPos);
        _shader.SetUniform("uScale", new Vector2(size, size));
        _shader.SetUniform("uScreenSize", _screenSize);
    
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        _gl.BindVertexArray(0);
    }
}