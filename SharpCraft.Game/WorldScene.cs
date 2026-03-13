using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering.Extra;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using SharpCraft.Engine.World.Blocks.GameReady;
using SharpCraft.Game.Screens;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace SharpCraft.Game;

public class WorldScene : IScene
{
    private GL _gl;
    private Shader _shader;
    private Block _grassBlock;
    private Block _dirtBlock;
    private WorldGenerator _worldGenerator;
    private static Canvas _activeCanvas;
    private PlayerController _playerController;

    public static GameWorld GameWorld;
    public static bool IsPaused { get; set; } = false;
    private static bool _isDebug = false;
    
    public static UIRenderer UIRenderer { get; private set; }

    public static Vector2 defaultButtonSize { get; } = new Vector2(350, 40);

    private readonly string _vertPath = Path.Combine("Shaders", "World", "block.vert");
    private readonly string _fragPath = Path.Combine("Shaders", "World", "block.frag");
    
    private BlockOutlineRenderer _outlineRenderer;
    private Shader _outlineShader;
    
    public void Load(UIRenderer uiRenderer, GL gl)
    {
        Console.WriteLine("[INFO] Loading Test World scene.");
        _gl = gl;
        _playerController = new PlayerController();
        _playerController.Load();
        UIRenderer = uiRenderer;
        
        _shader = new Shader(_gl, _vertPath, _fragPath);
        
        _grassBlock = new GrassBlock(_gl, _shader);
        _dirtBlock = new DirtBlock(_gl, _shader);
        
        _outlineShader = new Shader(_gl,
            Path.Combine("Shaders", "World", "outline.vert"),
            Path.Combine("Shaders", "World", "outline.frag"));
        _outlineRenderer = new BlockOutlineRenderer(_gl, _outlineShader);
        
        GameWorld = new GameWorld();
        _worldGenerator = new WorldGenerator();
        _worldGenerator.GenerateCube(GameWorld, 16, 16, 4, _grassBlock, _dirtBlock);
        
        GameWorld.AddBlock(3, 0, 0, _grassBlock);
        GameWorld.AddBlock(3, -1, 1, _grassBlock);
        
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
        _shader.SetUniform("uView", PlayerController.Camera.GetView());
        _shader.SetUniform("uProjection", PlayerController.Camera.GetProjection(aspect));

        foreach (var (model, block) in GameWorld.Blocks)
        {
            block.Draw(model);
        }
        
        _gl.Disable(EnableCap.DepthTest);
        
        HUD.Canvas.Render();
        
        if (_isDebug)
            DebugScreen.Canvas.Render();
        
        
        // Block outline in gameplay
        var hit = Raycast.Cast(PlayerController.Camera.Position, 
            PlayerController.Camera.Front, GameWorld);
        
        if (hit.HasValue)
        {
            _gl.Enable(EnableCap.DepthTest);
            var view = PlayerController.Camera.GetView();
            var proj = PlayerController.Camera.GetProjection(aspect);
            int[] vp = new int[4];
            _gl.GetInteger(GetPName.Viewport, vp);
            _outlineRenderer.DrawOutline(
                GameWorld.GetBlockAABB(hit.Value.model),
                new Vector4(0, 0, 0, 1),
                view, proj,
                new Vector2(vp[2], vp[3]));
            _gl.Disable(EnableCap.DepthTest);
        }
        
        _activeCanvas?.Render();
        _gl.Enable(EnableCap.DepthTest);
    }
    
    public void Update()
    {
        InputManager.ResetCursor();
        _playerController.Update();
        HUD.Update();

        if (InputManager.IsKeyJustPressed(Key.Escape))
        {
            TogglePause(PauseScreen.Canvas);
        }
        
        if (InputManager.IsKeyJustPressed(Key.F3))
            _isDebug = !_isDebug;
        
        if (_isDebug)
            DebugScreen.Update(PlayerController.Camera);
        
        _activeCanvas?.Update(UIRenderer);
    }

    public void Unload()
    {
        _gl.Disable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.PolygonOffsetFill);
        _activeCanvas.Clear();
        IsPaused = false;
        
        foreach (var (_, block) in GameWorld.Blocks)
            block.Dispose();
        
        GameWorld.Dispose();
        _shader.Dispose();
        _outlineShader.Dispose();
        _outlineRenderer.Dispose();
        
        _grassBlock?.Dispose();
        _dirtBlock?.Dispose();
        
        _activeCanvas = null;
        PlayerController.Camera = null;
        
        HUD.Unload();
        PauseScreen.Unload();
        OptionsScreen.Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        InputManager.UnlockMouse();
    }
    
    public static void ChangeScreen(Canvas canvas)
    {
        _activeCanvas = canvas;
    }
    
    public static void TogglePause(Canvas canvas)
    {
        IsPaused = !IsPaused;
        
        if (IsPaused)
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