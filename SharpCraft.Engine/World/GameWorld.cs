using SharpCraft.Engine.Physics;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.World;

public class GameWorld : IDisposable
{
    private readonly List<Block> _blockTypes = new();
    public List<(Matrix4X4<float> Model, Block Block)> Blocks = new();
    
    public void AddBlock(float x, float y, float z, Block block)
    {
        if (!_blockTypes.Contains(block))
            _blockTypes.Add(block);
        Blocks.Add((Matrix4X4.CreateTranslation<float>(x, y, z), block));
    }
    
    public void RemoveBlock(Matrix4X4<float> model)
    {
        Blocks.RemoveAll(b => 
            b.Model.M41 == model.M41 && 
            b.Model.M42 == model.M42 && 
            b.Model.M43 == model.M43);
    }
    
    public AABB GetBlockAABB(Matrix4X4<float> model)
    {
        var pos = new Vector3(model.M41, model.M42 - 0.5f, model.M43);
        return new AABB(pos, new Vector3(1f, 1f, 1f));
    }
    
    public void Dispose()
    {
        foreach (var block in _blockTypes)
            block.Dispose();
        Blocks.Clear();
        _blockTypes.Clear();
    }
}

