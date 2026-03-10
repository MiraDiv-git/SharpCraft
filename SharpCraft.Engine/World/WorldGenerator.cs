using Silk.NET.Maths;

namespace SharpCraft.Engine.World;

public class WorldGenerator
{
    public enum BlockType { Grass, Dirt }
    private readonly List<Matrix4X4<float>> _blockPositions = new();
    public List<(Matrix4X4<float> Model, BlockType Type)> Blocks = new();

    public void GenerateFlat(int width, int depth)
    {
        for (int x = 0; x < width; x++)
        for (int z = 0; z < depth; z++)
            _blockPositions.Add(Matrix4X4.CreateTranslation<float>(x, -2, z));
    }

    public void GenerateCube(int width, int depth, int height, BlockType topBlock, BlockType fillBlock)
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        for (int z = 0; z < depth; z++)
        {
            var type = y == height - 1 ? topBlock : fillBlock;
            Blocks.Add((Matrix4X4.CreateTranslation<float>(x, y - height - 1, z), type));
        }
    }

    public IReadOnlyList<Matrix4X4<float>> BlockPositions => _blockPositions;
}