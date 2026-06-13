using Silk.NET.Windowing;

namespace SharpCraft.Engine.Config;

public static class EngineDefaults
{
    public static class Window
    {
        public const string Title = "SharpCraft";
        public const int Width = 800;
        public const int Height = 600;
        public const bool VSync = false;

        public const WindowState Mode = WindowState.Normal; // Fullscreen | Minimized | Maximized | Normal*
        public const WindowBorder Border = WindowBorder.Resizable; // Fixed | Hidden | Resizable*
    }

    public static class Font
    {
        public static readonly string Path = System.IO.Path.Combine("Fonts", "dogicapixel.png");
        public const float Size = 16f;
        public static readonly Color4 Color = Rendering.Color.White;
        public const float Spacing = 0.4f;
        public const bool Shadow = true;
        public const float ShadowOffset = 2f;
        public const float VerticalOffset = 0f;
        public const UI.TextAlign TextAlign = UI.TextAlign.Center;
    }
}