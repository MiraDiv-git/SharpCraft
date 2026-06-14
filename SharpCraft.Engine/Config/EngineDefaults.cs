using Silk.NET.Windowing;

namespace SharpCraft.Engine.Config;

public static class EngineDefaults
{
    public static class Window
    {
        public static string Title = "SharpCraft";
        public static int Width = 800;
        public static int Height = 600;
        public static bool VSync = false;
        public static WindowState Mode = WindowState.Normal;
        public static WindowBorder Border = WindowBorder.Resizable;
    }

    public static class Font
    {
        public static string Path = System.IO.Path.Combine("Fonts", "dogicapixel.png");
        public static float Size = 16f;
        public static Color4 Color = Rendering.Color.White;
        public static float Spacing = 0.4f;
        public static bool Shadow = true;
        public static float ShadowOffset = 2f;
        public static float VerticalOffset = 0f;
        public static UI.TextAlign TextAlign = UI.TextAlign.Center;
    }
}