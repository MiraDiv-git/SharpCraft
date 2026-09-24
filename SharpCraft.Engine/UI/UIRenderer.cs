using FontStashSharp;
using FontStashSharp.Interfaces;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.UI;

public class UIRenderer : IDisposable
{
    private readonly GL _gl;
    private readonly Shader _shader;
    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly Vector2 _referenceSize;
    private Vector2 _screenSize;

    private FontSystem? _fontSystem;
    private GLFontTextureManager? _fontTextureManager;
    private UIFontRenderer? _fontRenderer;

    private readonly string _vertPath = Path.Combine("Shaders", "UI", "ui.vert");
    private readonly string _fragPath = Path.Combine("Shaders", "UI", "ui.frag");

    private readonly List<Canvas> _canvases = new();
    private readonly HashSet<Canvas> _canvasSet = new();

    // UI batch: 6 vertices per quad, 8 floats per vertex (ndcX, ndcY, u, v, r, g, b, a).
    private const int FloatsPerVertex = 8;
    private const int VerticesPerQuad = 6;
    private readonly List<float> _verts = new();
    private readonly List<int> _quadTextures = new();
    private readonly Dictionary<Texture, int> _textureSlots = new();
    private readonly List<Texture> _textures = new();
    private float[] _uploadBuffer = new float[4096];

    public Vector2 ScreenSize => _screenSize;
    public Vector2 ReferenceSize => _referenceSize;

    public static UIRenderer? Instance { get; private set; }

    public unsafe UIRenderer(GL gl, int width, int height)
    {
        Instance = this;
        _gl = gl;
        _referenceSize = new Vector2(width, height);
        _screenSize = new Vector2(width, height);

        _shader = new Shader(gl, _vertPath, _fragPath);

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, FloatsPerVertex * sizeof(float), (void*)0);   // NDC position
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, FloatsPerVertex * sizeof(float), (void*)(2 * sizeof(float))); // UV
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, FloatsPerVertex * sizeof(float), (void*)(4 * sizeof(float))); // Color
        _gl.EnableVertexAttribArray(2);

        _gl.BindVertexArray(0);
    }

    // --- Canvas management --- //

    public void RegisterCanvas(Canvas canvas)
    {
        if (_canvasSet.Add(canvas))
            _canvases.Add(canvas);
    }

    public void UnregisterCanvas(Canvas canvas)
    {
        if (_canvasSet.Remove(canvas))
            _canvases.Remove(canvas);
    }

    public void DeactivateAllCanvases()
    {
        foreach (var canvas in _canvases)
            canvas.SetActive(false);
    }

    // --- Frame --- //

    public void Update()
    {
        foreach (var canvas in _canvases.ToList())
            if (canvas.IsActive)
                canvas.Update(this);
    }

    public void Render()
    {
        foreach (var canvas in _canvases.ToList())
            if (canvas.IsActive)
                canvas.Render();

        Flush();
    }

    // --- Font --- //

    public void SetFont(byte[] ttfBytes)
    {
        _fontSystem?.Dispose();
        _fontTextureManager?.Dispose();
        _fontTextureManager = new GLFontTextureManager(_gl);

        _fontSystem = new FontSystem(new FontSystemSettings
        {
            PremultiplyAlpha = false,
            FontResolutionFactor = 1f
        });
        _fontSystem.AddFont(ttfBytes);

        _fontRenderer = new UIFontRenderer(this, _fontTextureManager);
    }

    public float DrawText(string text, Vector2 pixelPos, float fontSize, Color4 color,
        bool shadow = false, float shadowOffset = 0f)
    {
        if (_fontSystem == null) return 0f;

        var font = _fontSystem.GetFont(fontSize);
        var pos = new System.Numerics.Vector2(pixelPos.X, pixelPos.Y);

        if (shadow)
        {
            font.DrawText(_fontRenderer!, text, pos + new System.Numerics.Vector2(shadowOffset, shadowOffset),
                new FSColor(0f, 0f, 0f, color.a * 0.5f),
                rotation: 0f, origin: System.Numerics.Vector2.Zero, scale: null,
                layerDepth: 0f, characterSpacing: 0f, lineSpacing: 0f,
                textStyle: TextStyle.None, effect: FontSystemEffect.None, effectAmount: 0);
        }

        return font.DrawText(_fontRenderer!, text, pos,
            new FSColor(color.r, color.g, color.b, color.a),
            rotation: 0f, origin: System.Numerics.Vector2.Zero, scale: null,
            layerDepth: 0f, characterSpacing: 0f, lineSpacing: 0f,
            textStyle: TextStyle.None, effect: FontSystemEffect.None, effectAmount: 0);
    }

    public Vector2 MeasureText(string text, float fontSize)
    {
        if (_fontSystem == null) return Vector2.Zero;

        var measure = _fontSystem.GetFont(fontSize)
            .MeasureString(text, scale: null, characterSpacing: 0f, lineSpacing: 0f,
                effect: FontSystemEffect.None, effectAmount: 0);
        return new Vector2(measure.X, measure.Y);
    }

    public void SetScreenSize(int width, int height)
    {
        _screenSize = new Vector2(width, height);
    }

    // --- Layout --- //

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

    // --- Batched draw calls --- //

    private float ToNdcX(float px) => px / _screenSize.X * 2f - 1f;
    private float ToNdcY(float py) => 1f - py / _screenSize.Y * 2f;

    private void AddQuad(float nx0, float ny0, float nx1, float ny1,
        float u0, float v0, float u1, float v1, Color4 color, Texture? texture)
    {
        int slot = -1;
        if (texture != null)
        {
            if (!_textureSlots.TryGetValue(texture, out slot))
            {
                slot = _textures.Count;
                _textures.Add(texture);
                _textureSlots[texture] = slot;
            }
        }
        _quadTextures.Add(slot);

        float r = color.r, g = color.g, b = color.b, a = color.a;

        void Add(float nx, float ny, float u, float v)
        {
            _verts.Add(nx); _verts.Add(ny);
            _verts.Add(u); _verts.Add(v);
            _verts.Add(r); _verts.Add(g); _verts.Add(b); _verts.Add(a);
        }

        Add(nx0, ny0, u0, v0);
        Add(nx1, ny0, u1, v0);
        Add(nx1, ny1, u1, v1);
        Add(nx0, ny0, u0, v0);
        Add(nx1, ny1, u1, v1);
        Add(nx0, ny1, u0, v1);
    }

    private void AddRect(float px, float py, float pw, float ph,
        float u0, float v0, float u1, float v1, Color4 color, Texture? texture)
    {
        float nx0 = ToNdcX(px), ny0 = ToNdcY(py);
        float nx1 = ToNdcX(px + pw), ny1 = ToNdcY(py + ph);
        AddQuad(nx0, ny0, nx1, ny1, u0, v0, u1, v1, color, texture);
    }

    private void Flush()
    {
        if (_verts.Count == 0) return;

        _shader.Use();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        if (_uploadBuffer.Length < _verts.Count)
            _uploadBuffer = new float[_verts.Count];
        _verts.CopyTo(_uploadBuffer);
        unsafe
        {
            fixed (float* p = _uploadBuffer)
                _gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint)(_verts.Count * sizeof(float)), p, BufferUsageARB.DynamicDraw);
        }

        // Split into contiguous texture runs, preserving draw order.
        int quadCount = _quadTextures.Count;
        int start = 0;
        int curSlot = -1;
        for (int i = 0; i <= quadCount; i++)
        {
            int slot = i < quadCount ? _quadTextures[i] : -2;
            if (slot != curSlot)
            {
                FlushRange(start, i, curSlot);
                start = i;
                curSlot = slot;
            }
        }

        _gl.BindVertexArray(0);

        _verts.Clear();
        _quadTextures.Clear();
        _textureSlots.Clear();
        _textures.Clear();
    }

    private void FlushRange(int quadStart, int quadEnd, int slot)
    {
        if (quadStart >= quadEnd) return;

        int firstVertex = quadStart * VerticesPerQuad;
        int vertexCount = (quadEnd - quadStart) * VerticesPerQuad;

        if (slot >= 0)
        {
            _textures[slot].Bind();
            _shader.SetUniform("uUseTexture", 1);
        }
        else
        {
            _shader.SetUniform("uUseTexture", 0);
        }

        _gl.DrawArrays(PrimitiveType.Triangles, firstVertex, (uint)vertexCount);
    }

    // --- Public draw API (immediate mode, buffered hardware) --- //

    public void DrawRect(Vector2 position, Vector2 size, Color4 color, Anchor anchor)
    {
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        AddRect(resolvedPos.X, resolvedPos.Y, resolvedSize.X, resolvedSize.Y,
            0f, 0f, 1f, 1f, color, null);
    }

    public void DrawRectAbsolute(Vector2 screenPos, Vector2 screenSize, Color4 color)
    {
        AddRect(screenPos.X, screenPos.Y, screenSize.X, screenSize.Y,
            0f, 0f, 1f, 1f, color, null);
    }

    public void DrawTexturedRect(Vector2 position, Vector2 size, Texture texture, Color4 color, Anchor anchor)
    {
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        AddRect(resolvedPos.X, resolvedPos.Y, resolvedSize.X, resolvedSize.Y,
            0f, 0f, 1f, 1f, color, texture);
    }

    public void DrawTexturedRectAbsolute(Vector2 screenPos, Vector2 screenSize, Texture texture, Color4 color)
    {
        AddRect(screenPos.X, screenPos.Y, screenSize.X, screenSize.Y,
            0f, 0f, 1f, 1f, color, texture);
    }

    public void DrawTexturedRectUV(Vector2 position, Vector2 size, Texture texture,
        Color4 color, Anchor anchor, Vector2 uvOffset, Vector2 uvScale)
    {
        var resolvedSize = ResolveSize(size);
        var resolvedPos = ResolvePosition(position, resolvedSize, anchor);
        AddRect(resolvedPos.X, resolvedPos.Y, resolvedSize.X, resolvedSize.Y,
            uvOffset.X, uvOffset.Y, uvOffset.X + uvScale.X, uvOffset.Y + uvScale.Y, color, texture);
    }

    public void Dispose()
    {
        UnregisterAllCanvases();

        _fontSystem?.Dispose();
        _fontTextureManager?.Dispose();
        _fontSystem = null;

        _shader.Dispose();
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);

        if (Instance == this)
            Instance = null;
    }

    private void UnregisterAllCanvases()
    {
        foreach (var canvas in _canvases)
            canvas.SetActive(false);
        _canvasSet.Clear();
        _canvases.Clear();
    }

    // --- Runtime TTF plumbing --- //

    private sealed class GLFontTextureManager : ITexture2DManager, IDisposable
    {
        private readonly GL _gl;
        private readonly List<Texture> _textures = new();
        private readonly Dictionary<Texture, System.Drawing.Point> _sizes = new();
        private bool _disposed;

        public GLFontTextureManager(GL gl) => _gl = gl;

        public object CreateTexture(int width, int height)
        {
            var texture = Texture.Empty(_gl, width, height);
            _textures.Add(texture);
            _sizes[texture] = new System.Drawing.Point(width, height);
            return texture;
        }

        public System.Drawing.Point GetTextureSize(object texture) => _sizes[(Texture)texture];

        public void SetTextureData(object texture, System.Drawing.Rectangle bounds, byte[] data)
        {
            ((Texture)texture).UploadSubData(bounds.X, bounds.Y, bounds.Width, bounds.Height, data);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var texture in _textures)
                texture.Dispose();
            _textures.Clear();
            _sizes.Clear();
        }
    }

    private sealed class UIFontRenderer : IFontStashRenderer
    {
        private readonly UIRenderer _owner;
        private readonly GLFontTextureManager _textureManager;

        public UIFontRenderer(UIRenderer owner, GLFontTextureManager textureManager)
        {
            _owner = owner;
            _textureManager = textureManager;
        }

        public ITexture2DManager TextureManager => _textureManager;

        public void Draw(object texture, System.Numerics.Vector2 pos, System.Drawing.Rectangle? src, FSColor color,
            float rotation, System.Numerics.Vector2 scale, float depth)
        {
            if (src is not System.Drawing.Rectangle s) return;

            var size = _textureManager.GetTextureSize(texture);
            float w = s.Width * scale.X;
            float h = s.Height * scale.Y;

            _owner.AddRect(pos.X, pos.Y, w, h,
                s.X / (float)size.X, s.Y / (float)size.Y,
                (s.X + s.Width) / (float)size.X, (s.Y + s.Height) / (float)size.Y,
                (color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f),
                (Texture)texture);
        }
    }
}