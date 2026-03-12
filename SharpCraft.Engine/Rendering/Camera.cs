using SharpCraft.Engine.Physics;
using Silk.NET.Maths;

namespace SharpCraft.Engine.Rendering;

public class Camera
{
    public Vector3 Position { get; set; } = new(0, 0, 0);
    public float Yaw { get; set; } = -90f;
    public float Pitch { get; set; } = 0f;
    public float BaseFov { get; set; } = 70f;
    public float Fov { get; set; } = 70f;
    public float Speed { get; set; } = 5f;
    public float Sensitivity { get; set; } = 0.1f;
    
    private Vector3 _front = new(0, 0, -1);
    private Vector3 _up = new(0, 1, 0);
    
    public void Move(Vector3D<float> direction)
    {
        Position += direction * Speed * Time.DeltaTime;
    }
    
    public void Look(Vector2 delta)
    {
        Yaw += delta.X * Sensitivity;
        Pitch -= delta.Y * Sensitivity;
        Pitch = Math.Clamp(Pitch, -89f, 89f);
    }

    public Matrix4X4<float> GetView()
    {
        UpdateVectors();
        return Matrix4X4.CreateLookAt(Position, Position + _front, _up);
    }

    public Matrix4X4<float> GetProjection(float aspectRatio)
    {
        return Matrix4X4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(Fov), aspectRatio, 0.1f, 1000.0f);
    }

    private void UpdateVectors()
    {
        _front = Vector3D.Normalize(new Vector3(
            float.Cos(float.DegreesToRadians(Yaw)) * float.Cos(float.DegreesToRadians(Pitch)),
            float.Sin(float.DegreesToRadians(Pitch)),
            float.Sin(float.DegreesToRadians(Yaw)) * float.Cos(float.DegreesToRadians(Pitch))
            ));
    }
}