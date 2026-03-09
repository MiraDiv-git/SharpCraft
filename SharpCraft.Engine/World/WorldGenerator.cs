using Silk.NET.Maths;

namespace SharpCraft.Engine.World;

public class WorldGenerator
{
    private readonly List<Matrix4X4<float>> _blockPositions = new();

    public void GenerateFlat(int width, int depth)
    {
        for (int x = 0; x < width; x++)
        for (int z = 0; z < depth; z++)
            _blockPositions.Add(Matrix4X4.CreateTranslation<float>(x, -2, z));
    }

    public IReadOnlyList<Matrix4X4<float>> BlockPositions => _blockPositions;
}