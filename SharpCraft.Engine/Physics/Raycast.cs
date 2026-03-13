using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.Physics;

public class Raycast
{
    public static (Matrix4X4<float> model, Block block)? Cast(Vector3 origin, Vector3 direction, GameWorld world, float maxDistance = 5f)
    {
        var dir = Vector3D.Normalize(direction);
        
        for (float d = 0f; d < maxDistance; d += 0.05f)
        {
            var point = origin + dir * d;
            
            foreach (var (model, block) in world.Blocks)
            {
                var aabb = world.GetBlockAABB(model);
                if (point.X >= aabb.Min.X && point.X <= aabb.Max.X &&
                    point.Y >= aabb.Min.Y && point.Y <= aabb.Max.Y &&
                    point.Z >= aabb.Min.Z && point.Z <= aabb.Max.Z)
                    return (model, block);
            }
        }
        
        return null;
    }
}