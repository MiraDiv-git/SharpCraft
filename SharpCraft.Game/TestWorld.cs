using SharpCraft.Engine.Input;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;


namespace SharpCraft.Game;

public class TestWorld : IScene
{
    //private UIRenderer _uiRenderer;
    private GL _gl;
    private Shader _shader;
    private Camera _camera;
    private GrassBlock _block;
    private WorldGenerator _world;
    
    public void Load(UIRenderer uiRenderer, GL gl)
    {
        Console.WriteLine("[INFO] Loading Test World scene.");
        _gl = gl;

        _shader = new Shader(_gl, "Shaders/World/block.vert", "Shaders/World/block.frag");
        _camera = new Camera();
        
        _world = new WorldGenerator();
        _world.GenerateFlat(64, 64);
        _block = new GrassBlock(_gl, _shader);
        
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
        _shader.SetUniform("uView", _camera.GetView());
        _shader.SetUniform("uProjection", _camera.GetProjection(aspect));

        foreach (var model in _world.BlockPositions)
        {
            _block.Draw(model);
        }
        
        _block.Draw(Matrix4X4<float>.Identity);
    }
    
    public void Update()
    {
        InputManager.ResetCursor();
        ApplyMovement();

        if (InputManager.IsKeyDown(Key.Escape))
        {
            InputManager.UnlockMouse();
            SceneManager.SetScene(new MainMenuScene());
        }
    }

    public void Unload()
    {
        _gl.Disable(EnableCap.DepthTest);
        _gl.Disable(EnableCap.PolygonOffsetFill);
        InputManager.UnlockMouse();
    }

    public void ApplyMovement()
    {
        // Movement
        var forward = new Vector3D<float>(
            float.Cos(float.DegreesToRadians(_camera.Yaw)), 0,
            float.Sin(float.DegreesToRadians(_camera.Yaw)));
        var right = Vector3D.Normalize(Vector3D.Cross(forward, new Vector3D<float>(0, 1, 0)));
    
        var move = Vector3D<float>.Zero;
        if (InputManager.IsKeyDown(Key.W)) move += forward;
        if (InputManager.IsKeyDown(Key.S)) move -= forward;
        if (InputManager.IsKeyDown(Key.A)) move -= right;
        if (InputManager.IsKeyDown(Key.D)) move += right;
    
        if (move != Vector3D<float>.Zero)
            _camera.Move(Vector3D.Normalize(move));
        
        // Rotation
        _camera.Look(InputManager.MouseDelta);
    }
}