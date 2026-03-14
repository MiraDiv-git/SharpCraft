using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.Physics;

public class Raycast
{
    public static (Matrix4X4<float> model, Block block, Vector3 normal)? Cast(Vector3 origin, Vector3 direction, GameWorld world, float maxDistance = 5f)
    {
        var dir = Vector3D.Normalize(direction);
        
        for (float d = 0f; d < maxDistance; d += 0.05f)
        {
            var point = origin + dir * d;
            
            foreach (var (model, block) in world.Blocks)
            {
                var blockPos = new Vector3(model.M41, model.M42, model.M43);
                if (Vector3D.Distance(point, blockPos) > 1.5f) continue;
    
                var aabb = world.GetBlockAABB(model);
                if (point.X >= aabb.Min.X && point.X <= aabb.Max.X &&
                    point.Y >= aabb.Min.Y && point.Y <= aabb.Max.Y &&
                    point.Z >= aabb.Min.Z && point.Z <= aabb.Max.Z)
                {
                    var center = new Vector3(
                        (aabb.Min.X + aabb.Max.X) / 2f,
                        (aabb.Min.Y + aabb.Max.Y) / 2f,
                        (aabb.Min.Z + aabb.Max.Z) / 2f);
                    var diff = point - center;
                    float ax = MathF.Abs(diff.X);
                    float ay = MathF.Abs(diff.Y);
                    float az = MathF.Abs(diff.Z);
                    Vector3 normal;
                    if (ax > ay && ax > az)
                        normal = new Vector3(MathF.Sign(diff.X), 0, 0);
                    else if (ay > ax && ay > az)
                        normal = new Vector3(0, MathF.Sign(diff.Y), 0);
                    else
                        normal = new Vector3(0, 0, MathF.Sign(diff.Z));
                    return (model, block, normal);
                }
            }
        }
        
        return null;
    }
}