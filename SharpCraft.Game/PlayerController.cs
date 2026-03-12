using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace SharpCraft.Game;

public class PlayerController
{
    public Player Player;
    public static Camera Camera;
    
    private const float JumpForce = 8f;
    
    public void Load()
    {
        Player = new Player();
        Camera = new Camera();
    }

    public void Update()
    {
        if (!WorldScene.IsPaused)
        {
            Player.Update(Time.DeltaTime, WorldScene.GameWorld);
            Camera.Position = Player.Position + new Vector3(0, 1.6f, 0);
            ApplyMovement();
        }
    }
    public void ApplyMovement()
    {
        var forward = new Vector3(
            float.Cos(float.DegreesToRadians(Camera.Yaw)), 0,
            float.Sin(float.DegreesToRadians(Camera.Yaw)));
        var right = Vector3D.Normalize(Vector3D.Cross(forward, new Vector3(0, 1, 0)));
    
        var move = Vector3.Zero; // Movement
        if (InputManager.IsKeyDown(Key.W)) move += forward;
        if (InputManager.IsKeyDown(Key.S)) move -= forward;
        if (InputManager.IsKeyDown(Key.A)) move -= right;
        if (InputManager.IsKeyDown(Key.D)) move += right;

        if (InputManager.IsKeyDown(Key.Space) && Player.IsGrounded) // Jump
        {
            var vel = Player.Velocity;
            vel.Y = JumpForce;
            Player.Velocity = vel;
        }
        
        float targetFov = Camera.BaseFov; // Camera FOV
        if (move != Vector3.Zero)
        {
            var vel = Player.Velocity;
            var dir = Vector3D.Normalize(move);
            
            float currentSpeed = Camera.Speed;
            if (InputManager.IsKeyDown(Key.ControlLeft) && InputManager.IsKeyDown(Key.W)) // Sprinting
            {
                currentSpeed *= 1.5f;
                targetFov = Camera.BaseFov + 20f;
            }
            
            vel.X = dir.X * currentSpeed;
            vel.Z = dir.Z * currentSpeed;
            Player.Velocity = vel;
        }
        else
        {
            var vel = Player.Velocity;
            vel.X = 0; vel.Z = 0;
            Player.Velocity = vel;
            Camera.Fov += (Camera.BaseFov - Camera.Fov) * 10f * (float)Time.DeltaTime;
        }
        
        float lerpSpeed = 10f; 
        Camera.Fov += (targetFov - Camera.Fov) * lerpSpeed * (float)Time.DeltaTime;
        
        // Rotation
        Camera.Look(InputManager.MouseDelta);
    }
}