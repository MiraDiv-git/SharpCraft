namespace SharpCraft.Engine.Rendering;

public static class Color
{
    public static Color4 FromRGBA(byte r, byte g, byte b, byte a = 255)
        => (r / 255f, g / 255f, b / 255f, a / 255f);
    
    public static Color4 WithAlpha(this Color4 color, byte a)
        => (color.r, color.g, color.b, a / 255f);
    
    // --- Basic colors --- //
    
    // Standard
    public static Color4 Red => FromRGBA(255, 0, 0);
    public static Color4 Green => FromRGBA(0, 128, 0);
    public static Color4 Blue => FromRGBA(0, 0, 255);
    public static Color4 Yellow => FromRGBA(255, 255, 0);
    public static Color4 White => FromRGBA(255, 255, 255);
    public static Color4 Black => FromRGBA(0, 0, 0);
    
    // Color Variants
    public static Color4 Crimson => FromRGBA(220, 20, 60); 
    public static Color4 Lime => FromRGBA(0, 255, 0);
    public static Color4 Cyan => FromRGBA(0, 255, 255);
    public static Color4 Orange => FromRGBA(255, 165, 0);
    
    // Monochrome Variants
    public static Color4 Grey => FromRGBA(128, 128, 128);
    public static Color4 LightGrey => FromRGBA(169, 169, 169);
    public static Color4 DarkGrey => FromRGBA(30, 30, 30);
    
    // Other
    public static Color4 Sky => FromRGBA(128, 179, 255);
}