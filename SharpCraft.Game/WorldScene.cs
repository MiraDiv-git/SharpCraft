using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using SharpCraft.Engine.World.Blocks.GameReady;
using SharpCraft.Game.Screens;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;


namespace SharpCraft.Game;

public class WorldScene : IScene
{
    private GL _gl;
    private Shader _shader;
    private Block _grassBlock;
    private Block _dirtBlock;
    private WorldGenerator _world;
    private static Canvas _activeCanvas;
    private Player _player;
    
    private static bool _isPaused = false;
    private static bool _isDebug = false;
    
    
    public static UIRenderer UIRenderer { get; private set; }
    public static Camera Camera { get; set; }
    public static Vector2 defaultButtonSize { get; } = new Vector2(350, 40);

    private readonly string _vertPath = Path.Combine("Shaders", "World", "block.vert");
    private readonly string _fragPath = Path.Combine("Shaders", "World", "block.frag");
    
    private DebugRenderer _debugRenderer;
    private Shader _debugShader;
    
    public void Load(UIRenderer uiRenderer, GL gl)
    {
        Console.WriteLine("[INFO] Loading Test World scene.");
        _gl = gl;
        UIRenderer = uiRenderer;

        _shader = new Shader(_gl, _vertPath, _fragPath);
        Camera = new Camera();
        
        _player = new Player();
        
        _grassBlock = new GrassBlock(_gl, _shader);
        _dirtBlock = new DirtBlock(_gl, _shader);
        
        _debugShader = new Shader(_gl,
            Path.Combine("Shaders", "Debug", "Collisions", "collisions.vert"),
            Path.Combine("Shaders", "Debug", "Collisions", "collisions.frag"));
        _debugRenderer = new DebugRenderer(_gl, _debugShader);
        
        _world = new WorldGenerator();
        _world.GenerateCube(16, 16, 4, _grassBlock, _dirtBlock);
        
        _world.AddBlock(3, 0, 0, _grassBlock); //
        
        HUD.Load();
        DebugScreen.Load();
        PauseScreen.Load();
        
        InputManager.LockMouse();
    }

    public void Render()
    {
        var c = Color.Sky;
        _gl.ClearColor(c.r, c.g, c.b, c.a);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.Enable(EnableCap.DepthTest);
        _gl.Enable(EnableCap.PolygonOffsetFill);
        _gl.CullFace(TriangleFace.Back);
        
        int[] viewport = new int[4];
        _gl.GetInteger(GetPName.Viewport, viewport);
        float aspect = viewport[2] / (float)viewport[3];
        
        _shader.Use();
        _shader.SetUniform("uView", Camera.GetView());
        _shader.SetUniform("uProjection", Camera.GetProjection(aspect));

        foreach (var (model, block) in _world.Blocks)
        {
            block.Draw(model);
        }
        
        _gl.Disable(EnableCap.DepthTest);
        
        HUD.Canvas.Render();
        
        if (_isDebug)
        {
            DebugScreen.Canvas.Render();
            
            var view = Camera.GetView();
            var proj = Camera.GetProjection(aspect);
            
            foreach (var (model, _) in _world.Blocks)
                _debugRenderer.DrawAABB(_world.GetBlockAABB(model), new Vector3(1, 0, 0), view, proj);
        }
        
        _activeCanvas?.Render();
        _gl.Enable(EnableCap.DepthTest);
    }
    
    public void Update()
    {
        InputManager.ResetCursor();
        HUD.Update();

        if (!_isPaused)
        {
            _player.Update(Time.DeltaTime, _world);
            Camera.Position = _player.Position + new Vector3(0, 1.6f, 0);
            ApplyMovement();
        }

        if (InputManager.IsKeyJustPressed(Key.Escape))
        {
            TogglePause(PauseScreen.Canvas);
        }
        
        if (InputManager.IsKeyJustPressed(Key.F3))
            _isDebug = !_isDebug;
        
        if (_isDebug)
            DebugScreen.Update(Camera);
        
        _activeCanvas?.Update(UIRenderer);
    }

    public void Unload()
    {
        _gl.Disable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.PolygonOffsetFill);
        _activeCanvas.Clear();
        _isPaused = false;
        InputManager.UnlockMouse();
    }

    private void ApplyMovement()
    {
        // Movement
        var forward = new Vector3(
            float.Cos(float.DegreesToRadians(Camera.Yaw)), 0,
            float.Sin(float.DegreesToRadians(Camera.Yaw)));
        var right = Vector3D.Normalize(Vector3D.Cross(forward, new Vector3(0, 1, 0)));
    
        var move = Vector3.Zero;
        if (InputManager.IsKeyDown(Key.W)) move += forward;
        if (InputManager.IsKeyDown(Key.S)) move -= forward;
        if (InputManager.IsKeyDown(Key.A)) move -= right;
        if (InputManager.IsKeyDown(Key.D)) move += right;
    
        if (move != Vector3D<float>.Zero)
        {
            var vel = _player.Velocity;
            var dir = Vector3D.Normalize(move);
            vel.X = dir.X * Camera.Speed;
            vel.Z = dir.Z * Camera.Speed;
            _player.Velocity = vel;
        }
        else
        {
            var vel = _player.Velocity;
            vel.X = 0; vel.Z = 0;
            _player.Velocity = vel;
        }
        
        // Rotation
        Camera.Look(InputManager.MouseDelta);
    }
    
    public static void ChangeScreen(Canvas canvas)
    {
        _activeCanvas = canvas;
    }
    
    public static void TogglePause(Canvas canvas)
    {
        _isPaused = !_isPaused;
        
        if (_isPaused)
        {
            _activeCanvas = canvas;
            InputManager.UnlockMouse();
        }
        else
        {
            _activeCanvas = null;
            InputManager.LockMouse();
        }
    }
}