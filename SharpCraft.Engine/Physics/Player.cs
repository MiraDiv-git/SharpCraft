using Silk.NET.Maths;
using SharpCraft.Engine.World;
namespace SharpCraft.Engine.Physics;

public class Player
{
    public Vector3 Position { get; set; } = new(0, -2, 0);
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    public bool IsGrounded { get; private set; }
    public bool IsFlying { get; set; } = false;

    private static readonly Vector3 Size = new(0.6f, 1.8f, 0.6f);
    private const float Gravity = -25f;

    public AABB GetAABB() => new(Position, Size);

    public void Update(float deltaTime, GameWorld world) // Gravity
    {
        IsGrounded = false;
        
        var vel = Velocity;
        vel.Y += Gravity * deltaTime;
        Velocity = vel;

        MoveAxis(ref vel, 0, deltaTime, world); // X
        MoveAxis(ref vel, 1, deltaTime, world); // Y
        MoveAxis(ref vel, 2, deltaTime, world); // Z
        
        Velocity = vel;

        if (!IsFlying)
            vel.Y -= Gravity * deltaTime;
        else
            vel.Y = 0;
    }

    private void MoveAxis(ref Vector3 vel, int axis, float dt, GameWorld world)
    {
        var pos = Position;
        if (axis == 0) pos.X += vel.X * dt;
        if (axis == 1) pos.Y += vel.Y * dt;
        if (axis == 2) pos.Z += vel.Z * dt;
        Position = pos;

        foreach (var (model, _) in world.Blocks)
        {
            var blockPos = new Vector3(model.M41, model.M42, model.M43);
            var blockAABB = world.GetBlockAABB(model);

            if (!GetAABB().Intersects(blockAABB)) continue;

            pos = Position;
            if (axis == 0) { pos.X = vel.X > 0 ? blockAABB.Min.X - Size.X / 2 : blockAABB.Max.X + Size.X / 2; vel.X = 0; }
            if (axis == 1) { IsGrounded = vel.Y < 0; pos.Y = vel.Y > 0 ? blockAABB.Min.Y - Size.Y : blockAABB.Max.Y; vel.Y = 0; }
            if (axis == 2) { pos.Z = vel.Z > 0 ? blockAABB.Min.Z - Size.Z / 2 : blockAABB.Max.Z + Size.Z / 2; vel.Z = 0; }
            Position = pos;
            break;
        }
    }
}