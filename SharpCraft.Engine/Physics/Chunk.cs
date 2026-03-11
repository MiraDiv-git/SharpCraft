namespace SharpCraft.Engine.Physics;

public class Chunk 
{
    public const int Width = 16, Height = 256, Depth = 16;
    public byte[] Blocks = new byte[Width * Height * Depth];

    public bool IsSolid(int x, int y, int z) 
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth) return false;
        return Blocks[(x * Depth + z) * Height + y] != 0; 
    }
    
    public bool IsInBounds(int x, int y, int z) =>
        x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth;
}