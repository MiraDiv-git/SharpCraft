using Silk.NET.Core;
using Silk.NET.Windowing;
using StbImageSharp;

namespace SharpCraft.Engine.Rendering;

public static class WindowIcon
{
    public static unsafe void Set(IWindow window, string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(0);
        var image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        var icon = new RawImage(image.Width, image.Height, new Memory<byte>(image.Data));
        window.SetWindowIcon(ref icon);
    }
}