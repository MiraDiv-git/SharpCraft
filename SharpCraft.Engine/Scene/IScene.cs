namespace SharpCraft.Engine.Scene;

public interface IScene : IDisposable
{
    // Required
    void Load();

    // Optional
    void Render() { }
    void Unload() { }
    void Update() { }

    void IDisposable.Dispose() => Unload();
}