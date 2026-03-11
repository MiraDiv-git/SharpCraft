namespace SharpCraft.Engine.Physics;

public class AABB
{
    public Vector3 Min, Max;
    
    public AABB(Vector3 pos, Vector3 size) 
    {
        Min = pos - new Vector3(size.X / 2, 0, size.Z / 2);
        Max = pos + new Vector3(size.X / 2, size.Y, size.Z / 2);
    }
    
    public bool Intersects(AABB other) =>
        Max.X > other.Min.X && Min.X < other.Max.X &&
        Max.Y > other.Min.Y && Min.Y < other.Max.Y &&
        Max.Z > other.Min.Z && Min.Z < other.Max.Z;
}