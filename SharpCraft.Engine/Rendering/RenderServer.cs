using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Rendering;

public sealed class RenderServer : IDisposable
{
    private readonly List<IDisposable> _tracked = new();

    public GL Gl { get; }

    public static RenderServer? Instance { get; private set; }

    public RenderServer(GL gl)
    {
        Gl = gl;
        Instance = this;
    }

    /// <summary>
    /// Registers a GPU resource so it is guaranteed to be disposed at shutdown.
    /// </summary>
    public T Track<T>(T resource) where T : IDisposable
    {
        _tracked.Add(resource);
        return resource;
    }

    public void BeginFrame(Color4 clearColor)
    {
        Gl.ClearColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        BeginWorld();
    }

    public void BeginWorld()
    {
        Gl.Enable(EnableCap.DepthTest);
        Gl.DepthFunc(DepthFunction.Less);
        Gl.Disable(EnableCap.Blend);
    }

    public void BeginUI()
    {
        Gl.Disable(EnableCap.DepthTest);
        Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void Dispose()
    {
        for (int i = _tracked.Count - 1; i >= 0; i--)
            _tracked[i].Dispose();
        _tracked.Clear();

        if (Instance == this)
            Instance = null;
    }
}