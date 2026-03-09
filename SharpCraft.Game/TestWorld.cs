using SharpCraft.Engine.Assets;
using SharpCraft.Engine.Audio;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Rendering;
using SharpCraft.Engine.Scene;
using SharpCraft.Engine.UI;
using SharpCraft.Engine.UI.Elements;
using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks.GameReady;
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
    
    private UIRenderer _uiRenderer;
    private Canvas _activeCanvas;
    private Canvas _pauseMenuCanvas;
    
    private Texture _buttonTexture;
    private Texture _buttonHoverTexture;
    
    private Sound _clickSound;

    private bool _isPaused = false;
    
    private Vector2 defaultButtonSize = new Vector2(350, 40);
    
    public void Load(UIRenderer uiRenderer, GL gl)
    {
        Console.WriteLine("[INFO] Loading Test World scene.");
        _gl = gl;
        _uiRenderer = uiRenderer;

        _shader = new Shader(_gl, "Shaders/World/block.vert", "Shaders/World/block.frag");
        _camera = new Camera();
        
        _world = new WorldGenerator();
        _world.GenerateFlat(64, 64);
        _block = new GrassBlock(_gl, _shader);

        _pauseMenuCanvas = new Canvas(_uiRenderer);

        _clickSound = AudioManager.LoadAudio("Sounds/UI/click_ui.ogg");
        
        _buttonTexture = AssetManager.LoadTexture("Textures/UI/Button/button.png");
        _buttonHoverTexture = AssetManager.LoadTexture("Textures/UI/Button/button_hover.png");

        LoadPauseMenu();
        
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
        
        _gl.Disable(EnableCap.DepthTest);
        _activeCanvas?.Render();
        _gl.Enable(EnableCap.DepthTest);
    }
    
    public void Update()
    {
        InputManager.ResetCursor();
        
        if (!_isPaused)
            ApplyMovement();

        if (InputManager.IsKeyJustPressed(Key.Escape))
        {
            TogglePause(_pauseMenuCanvas);
        }
        
        _activeCanvas?.Update(_uiRenderer);
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
    
    private void TogglePause(Canvas canvas)
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

    public void LoadPauseMenu()
    {
        // Background
        var bgimage = _pauseMenuCanvas.AddElement<UIImage>();
        bgimage.Size = new Vector2(9999, 9999);
        bgimage.Anchor = Anchor.MiddleCenter;
        bgimage.ImageColor = Color.Black.WithAlpha(200);
        
        // Resume button
        var resbut = _pauseMenuCanvas.AddElement<UIButton>();
        resbut.Position = new Vector2(0, 0);
        resbut.Size = defaultButtonSize;
        resbut.Anchor = Anchor.MiddleCenter;
        resbut.ButtonColor = Color.White;
        resbut.HoverColor = Color.White;
        resbut.ButtonTexture = _buttonTexture;
        resbut.HoverTexture = _buttonHoverTexture;
        resbut.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            TogglePause(_pauseMenuCanvas);
        };
        
        // Resume button text
        var resbuttxt = _pauseMenuCanvas.AddElement<UIText>();
        resbuttxt.Position = resbut.Position;
        resbuttxt.Anchor = resbut.Anchor;
        resbuttxt.TextColor = Color.White;
        resbuttxt.Text = "Resume";
        
        
        // Main Menu button
        var menubutton = _pauseMenuCanvas.AddElement<UIButton>();
        menubutton.Position = new Vector2(resbut.Position.X, resbut.Position.Y + 50);
        menubutton.Size = defaultButtonSize;
        menubutton.Anchor = Anchor.MiddleCenter;
        menubutton.ButtonColor = Color.White;
        menubutton.HoverColor = Color.White;
        menubutton.ButtonTexture = _buttonTexture;
        menubutton.HoverTexture = _buttonHoverTexture;
        menubutton.OnClick += () =>
        {
            AudioManager.Play(_clickSound);
            SceneManager.SetScene(new MainMenuScene());
        };
        
        // Main Menu button text
        var menubuttxt = _pauseMenuCanvas.AddElement<UIText>();
        menubuttxt.Position = menubutton.Position;
        menubuttxt.Anchor = menubutton.Anchor;
        menubuttxt.TextColor = Color.White;
        menubuttxt.Text = "Exit";
    }
}