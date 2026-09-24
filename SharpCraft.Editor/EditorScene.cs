using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Rendering.World;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace SharpCraft.Editor;

public class EditorScene : IScene
{
    // Block palette ids.
    private const byte Grass = 1, Dirt = 2, Stone = 3, Sand = 4;

    private readonly Camera _camera = new()
    {
        Position = new Vector3(0f, 34f, 30f),
        Yaw = -90f,
        Pitch = -18f,
        Speed = 12f
    };

    private WorldRenderer? _world;
    private Shader? _chunkShader;
    private Canvas? _ui;

    private readonly List<UIElement> _hudElements = new();
    private bool _hudVisible = true;

    private UIText? _posText;
    private UIText? _chunkText;
    private UIText? _fpsText;
    private UIText? _modeText;

    private int _frameCount;
    private float _fpsTimer;
    private float _fps;

    public void Load()
    {
        var server = RenderServer.Instance!;
        var gl = server.Gl;

        _chunkShader = new Shader(gl, "Shaders/World/chunk.vert", "Shaders/World/chunk.frag");
        server.Track(_chunkShader);

        _world = new WorldRenderer(gl);
        server.Track(_world);

        GenerateWorld();
        BuildUI();

        Console.WriteLine("[EDITOR] Scene loaded.");
        Console.WriteLine("\t├─ Chunks: 3x1x3");
        Console.WriteLine("\t└─ Controls: WASD move, Space up, Ctrl down, Shift fast, RMB look");
    }

    private void GenerateWorld()
    {
        for (int cx = -1; cx <= 1; cx++)
        for (int cz = -1; cz <= 1; cz++)
        {
            var coord = new Vector3D<int>(cx, 0, cz);
            _world!.SetChunk(coord, BuildChunk(cx, cz), Palette);
        }
    }

    private static Chunk BuildChunk(int cx, int cz)
    {
        var chunk = new Chunk();
        int baseX = cx * Chunk.Width;
        int baseZ = cz * Chunk.Depth;

        for (int x = 0; x < Chunk.Width; x++)
        for (int z = 0; z < Chunk.Depth; z++)
        {
            int wx = baseX + x;
            int wz = baseZ + z;

            float h = 18f
                      + MathF.Sin(wx * 0.22f) * 3f
                      + MathF.Cos(wz * 0.22f) * 3f
                      + MathF.Sin((wx + wz) * 0.07f) * 4f;
            int height = (int)MathF.Round(h);
            height = Math.Clamp(height, 2, Chunk.Height - 1);

            bool lowland = height <= 12;
            byte top = lowland ? Sand : Grass;

            for (int y = 0; y < height; y++)
            {
                byte id;
                if (y == height - 1)      id = top;
                else if (y >= height - 3) id = Dirt;
                else                      id = Stone;

                chunk.Blocks[(x * Chunk.Depth + z) * Chunk.Height + y] = id;
            }
        }

        return chunk;
    }

    private static (Vector3 color, bool solid) Palette(byte id) => id switch
    {
        Grass => (new Vector3(0.28f, 0.62f, 0.22f), true),
        Dirt  => (new Vector3(0.55f, 0.42f, 0.28f), true),
        Stone => (new Vector3(0.62f, 0.62f, 0.66f), true),
        _     => (new Vector3(0.92f, 0.87f, 0.62f), true)
    };

    private void BuildUI()
    {
        _ui = new Canvas();
        _ui.SetActive(true);

        T Add<T>() where T : UIElement, new()
        {
            var element = _ui.AddElement<T>();
            _hudElements.Add(element);
            return element;
        }

        // Top bar.
        var topBar = Add<UIPanel>();
        topBar.Position = new Vector2(0, 0);
        topBar.Size = new Vector2(1280, 48);
        topBar.Border = true;
        topBar.BorderThickness = 2f;

        var title = Add<UIText>();
        title.Position = new Vector2(16, 10);
        title.Size = new Vector2(600, 28);
        title.Text = "SharpCraft Editor — World View";
        title.FontSize = 20f;
        title.Shadow = true;
        title.FontColor = (0.95f, 0.95f, 1f, 1f);

        // Left: hierarchy placeholder.
        var scenesPanel = Add<UIPanel>();
        scenesPanel.Position = new Vector2(12, 60);
        scenesPanel.Size = new Vector2(240, 300);

        var scenesHeader = Add<UIText>();
        scenesHeader.Position = new Vector2(24, 68);
        scenesHeader.Size = new Vector2(216, 22);
        scenesHeader.Text = "Scenes / Hierarchy";
        scenesHeader.FontSize = 16f;
        scenesHeader.FontColor = (0.85f, 0.85f, 0.95f, 1f);

        var scenesEmpty = Add<UIText>();
        scenesEmpty.Position = new Vector2(24, 96);
        scenesEmpty.Size = new Vector2(216, 20);
        scenesEmpty.Text = "(empty)";
        scenesEmpty.FontSize = 13f;
        scenesEmpty.FontColor = (0.5f, 0.5f, 0.6f, 1f);

        // Left-bottom: properties placeholder.
        var propsPanel = Add<UIPanel>();
        propsPanel.Position = new Vector2(12, 372);
        propsPanel.Size = new Vector2(240, 140);

        var propsHeader = Add<UIText>();
        propsHeader.Position = new Vector2(24, 380);
        propsHeader.Size = new Vector2(216, 22);
        propsHeader.Text = "Properties";
        propsHeader.FontSize = 16f;
        propsHeader.FontColor = (0.85f, 0.85f, 0.95f, 1f);

        var propsEmpty = Add<UIText>();
        propsEmpty.Position = new Vector2(24, 408);
        propsEmpty.Size = new Vector2(216, 20);
        propsEmpty.Text = "(select an object)";
        propsEmpty.FontSize = 13f;
        propsEmpty.FontColor = (0.5f, 0.5f, 0.6f, 1f);

        // Right: statistics panel.
        var statsPanel = Add<UIPanel>();
        statsPanel.Anchor = Anchor.TopRight;
        statsPanel.Position = new Vector2(-12, 60);
        statsPanel.Size = new Vector2(300, 180);

        var statsHeader = Add<UIText>();
        statsHeader.Anchor = Anchor.TopRight;
        statsHeader.Position = new Vector2(-292, 68);
        statsHeader.Size = new Vector2(280, 22);
        statsHeader.Text = "Statistics";
        statsHeader.FontSize = 16f;
        statsHeader.FontColor = (0.95f, 0.95f, 1f, 1f);

        _posText = Add<UIText>();
        _posText.Anchor = Anchor.TopRight;
        _posText.Position = new Vector2(-292, 96);
        _posText.Size = new Vector2(280, 20);
        _posText.Text = "X: 0.0  Y: 0.0  Z: 0.0";
        _posText.FontSize = 13f;
        _posText.FontColor = (0.75f, 0.85f, 1f, 1f);

        _chunkText = Add<UIText>();
        _chunkText.Anchor = Anchor.TopRight;
        _chunkText.Position = new Vector2(-292, 120);
        _chunkText.Size = new Vector2(280, 20);
        _chunkText.FontSize = 13f;
        _chunkText.FontColor = (0.75f, 0.85f, 1f, 1f);

        _fpsText = Add<UIText>();
        _fpsText.Anchor = Anchor.TopRight;
        _fpsText.Position = new Vector2(-292, 144);
        _fpsText.Size = new Vector2(280, 20);
        _fpsText.Text = "FPS: 0";
        _fpsText.FontSize = 13f;
        _fpsText.FontColor = (0.75f, 0.85f, 1f, 1f);

        _modeText = Add<UIText>();
        _modeText.Anchor = Anchor.TopRight;
        _modeText.Position = new Vector2(-292, 168);
        _modeText.Size = new Vector2(280, 20);
        _modeText.FontSize = 13f;
        _modeText.FontColor = (0.75f, 0.75f, 0.85f, 1f);

        var hint = Add<UIText>();
        hint.Anchor = Anchor.BottomCenter;
        hint.Position = new Vector2(0, -12);
        hint.Size = new Vector2(900, 20);
        hint.Text = "WASD — move   Space — up   Ctrl — down   Shift — fast   Hold RMB — look around   Esc — release mouse   F1 — toggle HUD";
        hint.FontSize = 13f;
        hint.Shadow = true;
        hint.FontColor = (0.8f, 0.8f, 0.85f, 1f);
    }

    public void Update()
    {
        HandleCamera();
        UpdateStats();
    }

    private void HandleCamera()
    {
        if (InputManager.IsKeyJustPressed(Key.F1))
        {
            _hudVisible = !_hudVisible;
            foreach (var element in _hudElements)
                element.Visible = _hudVisible;
        }

        if (InputManager.RightMouseButtonJustPressed)
            InputManager.LockMouse();
        if (InputManager.RightMouseButtonJustReleased)
            InputManager.UnlockMouse();
        if (InputManager.IsKeyJustPressed(Key.Escape))
            InputManager.UnlockMouse();

        if (InputManager.IsMouseLocked)
            _camera.Look(InputManager.MouseDelta);

        float speed = _camera.Speed;
        if (InputManager.IsKeyDown(Key.ShiftLeft) || InputManager.IsKeyDown(Key.ShiftRight))
            speed *= 3f;

        var front = _camera.Front;
        var right = Vector3D.Normalize(Vector3D.Cross(front, new Vector3(0, 1, 0)));
        var horizontal = new Vector3(front.X, 0, front.Z);

        Vector3 move = Vector3.Zero;
        if (InputManager.IsKeyDown(Key.W)) move += horizontal;
        if (InputManager.IsKeyDown(Key.S)) move -= horizontal;
        if (InputManager.IsKeyDown(Key.D)) move += right;
        if (InputManager.IsKeyDown(Key.A)) move -= right;
        if (move.LengthSquared > 0f)
        {
            move = Vector3D.Normalize(move);
            _camera.Position += new Vector3(move.X, 0, move.Z) * speed * Time.DeltaTime;
        }

        if (InputManager.IsKeyDown(Key.Space))    _camera.Position += new Vector3(0, 1, 0) * speed * Time.DeltaTime;
        if (InputManager.IsKeyDown(Key.ControlLeft)) _camera.Position -= new Vector3(0, 1, 0) * speed * Time.DeltaTime;
    }

    private void UpdateStats()
    {
        if (_posText == null) return;

        var pos = _camera.Position;
        _posText!.Text = $"X: {pos.X,7:F1}   Y: {pos.Y,7:F1}   Z: {pos.Z,7:F1}";

        var world = _world;
        _chunkText!.Text = $"Chunks: {world?.ChunkCount ?? 0}";

        _frameCount++;
        _fpsTimer += Time.DeltaTime;
        if (_fpsTimer >= 0.25f)
        {
            _fps = _frameCount / _fpsTimer;
            _frameCount = 0;
            _fpsTimer = 0f;
            _fpsText!.Text = $"FPS: {_fps,5:F1}";
        }

        _modeText!.Text = InputManager.IsMouseLocked
            ? "Mouse: LOCKED (Esc to release)"
            : "Mouse: free (hold RMB to look)";
    }

    public void Render()
    {
        var screen = UIRenderer.Instance?.ScreenSize ?? Vector2.Zero;
        float aspect = screen.Y > 0f ? screen.X / screen.Y : 16f / 9f;

        _world?.Draw(_chunkShader!, _camera.GetView(), _camera.GetProjection(aspect));
    }

    public void Unload()
    {
        _ui?.Dispose();
        _ui = null;
        _world = null;
        _chunkShader = null;

        _posText = null;
        _chunkText = null;
        _fpsText = null;
        _modeText = null;

        Console.WriteLine("[EDITOR] Scene unloaded.");
    }
}