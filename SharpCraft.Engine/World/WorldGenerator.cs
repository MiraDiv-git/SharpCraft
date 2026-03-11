using SharpCraft.Engine.Physics;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.World;

public class WorldGenerator
{
    private readonly List<Matrix4X4<float>> _blockPositions = new();
    public List<(Matrix4X4<float> Model, Block Block)> Blocks = new();

    public void GenerateFlat(int width, int depth)
    {
        for (int x = 0; x < width; x++)
        for (int z = 0; z < depth; z++)
            _blockPositions.Add(Matrix4X4.CreateTranslation<float>(x, -2, z));
    }

    public void GenerateCube(int width, int depth, int height, Block topBlock, Block fillBlock)
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        for (int z = 0; z < depth; z++)
        {
            var type = y == height - 1 ? topBlock : fillBlock;
            Blocks.Add((Matrix4X4.CreateTranslation<float>(x - width / 2, 
                y - height - 1, 
                z - depth / 2), 
                type));
        }
    }
    
    public AABB GetBlockAABB(Matrix4X4<float> model)
    {
        var blockPos = new Vector3(model.M41, model.M42, model.M43);
        return new AABB(blockPos - new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f));
    }
    
    public void AddBlock(float x, float y, float z, Block block)
    {
        Blocks.Add((Matrix4X4.CreateTranslation<float>(x, y, z), block));
    }

    public IReadOnlyList<Matrix4X4<float>> BlockPositions => _blockPositions;
}