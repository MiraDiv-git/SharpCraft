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
    
    private const float JumpForce = 7.5f;
    
    private float _lastSpaceTime = -1f;
    private const float DoubleClickThreshold = 0.3f;
    
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
            
            if (Player.IsFlying && Player.IsGrounded)
                Player.IsFlying = false;
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

        if (InputManager.IsKeyJustPressed(Key.Space)) // Fly check
        {
            float now = (float)Time.TotalTime;
            if (now - _lastSpaceTime < DoubleClickThreshold)
            {
                Player.IsFlying = !Player.IsFlying;
                _lastSpaceTime = -1f;
            }
            else
            {
                _lastSpaceTime = now;
            }
        }
        
        if (InputManager.IsKeyDown(Key.Space) && Player.IsGrounded && !Player.IsFlying) // Jump
        {
            var vel = Player.Velocity;
            vel.Y = JumpForce;
            Player.Velocity = vel;
        }

        if (Player.IsFlying) // Flying
        {
            var vel = Player.Velocity;
            vel.Y = 0;
            if (InputManager.IsKeyDown(Key.Space)) vel.Y = Camera.Speed * 2f;
            if (InputManager.IsKeyDown(Key.ShiftLeft)) vel.Y = -Camera.Speed * 2f;
            Player.Velocity = vel;
        }
        
        float targetFov = Camera.BaseFov; // Camera FOV
        if (move != Vector3.Zero) // Check if player is moving
        {
            var vel = Player.Velocity;
            var dir = Vector3D.Normalize(move);
            
            float currentSpeed = Player.IsFlying ? Camera.Speed * 2f : Camera.Speed;
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