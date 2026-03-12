using SharpCraft.Engine.Physics;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.World;

public class WorldGenerator
{
    public void GenerateCube(GameWorld gameWorld, int width, int depth, int height, Block topBlock, Block fillBlock)
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        for (int z = 0; z < depth; z++)
        {
            var type = y == height - 1 ? topBlock : fillBlock;
            gameWorld.AddBlock(x - width / 2, y - height - 1, z - depth / 2, type);
        }
    }
}