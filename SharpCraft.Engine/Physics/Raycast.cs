using SharpCraft.Engine.World;
using SharpCraft.Engine.World.Blocks;
using Silk.NET.Maths;

namespace SharpCraft.Engine.Physics;

public class Raycast
{
    public static (Matrix4X4<float> model, Block block, Vector3 normal)? Cast(Vector3 origin, Vector3 direction, GameWorld world, float maxDistance = 5f)
    {
        var dir = Vector3D.Normalize(direction);
        
        (Matrix4X4<float> model, Block block, Vector3 normal, float dist)? closest = null;

        foreach (var (model, block) in world.Blocks)
        {
            var blockPos = new Vector3(model.M41, model.M42, model.M43);
            if (Vector3D.Distance(origin, blockPos) > maxDistance + 2f) continue;

            var aabb = world.GetBlockAABB(model);
            var hit = RayIntersectsAABB(origin, dir, aabb);
            if (hit.HasValue && hit.Value < maxDistance)
            {
                if (closest == null || hit.Value < closest.Value.dist)
                {
                    var point = origin + dir * hit.Value;
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
                    closest = (model, block, normal, hit.Value);
                }
            }
        }

        return closest.HasValue ? (closest.Value.model, closest.Value.block, closest.Value.normal) : null;
    }

    private static float? RayIntersectsAABB(Vector3 origin, Vector3 dir, AABB aabb)
    {
        float tmin = (aabb.Min.X - origin.X) / dir.X;
        float tmax = (aabb.Max.X - origin.X) / dir.X;
        if (tmin > tmax) (tmin, tmax) = (tmax, tmin);

        float tymin = (aabb.Min.Y - origin.Y) / dir.Y;
        float tymax = (aabb.Max.Y - origin.Y) / dir.Y;
        if (tymin > tymax) (tymin, tymax) = (tymax, tymin);

        if (tmin > tymax || tymin > tmax) return null;
        tmin = MathF.Max(tmin, tymin);
        tmax = MathF.Min(tmax, tymax);

        float tzmin = (aabb.Min.Z - origin.Z) / dir.Z;
        float tzmax = (aabb.Max.Z - origin.Z) / dir.Z;
        if (tzmin > tzmax) (tzmin, tzmax) = (tzmax, tzmin);

        if (tmin > tzmax || tzmin > tmax) return null;
        tmin = MathF.Max(tmin, tzmin);

        return tmin >= 0 ? tmin : null;
    }
}