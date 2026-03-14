using SharpCraft.Engine;
using SharpCraft.Engine.Input;
using SharpCraft.Engine.Physics;
using SharpCraft.Engine.Rendering;
using Silk.NET.Maths;

namespace SharpCraft.Game;

public class PlayerController
{
    public Player Player;
    public static Camera Camera;
    
    private const float JumpForce = 7.5f;
    
    private float _lastSpaceTime = -1f;
    private const float DoubleClickThreshold = 0.3f;
    private float _blockActionTimer = 0f;
    private const float BlockActionDelay = 0.2f;
    
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
    
    public void ResetBlockTimer()
    {
        _blockActionTimer = BlockActionDelay;
    }
    
    public void ApplyMovement()
    {
        var forward = new Vector3(
            float.Cos(float.DegreesToRadians(Camera.Yaw)), 0,
            float.Sin(float.DegreesToRadians(Camera.Yaw)));
        var right = Vector3D.Normalize(Vector3D.Cross(forward, new Vector3(0, 1, 0)));
    
        var move = Vector3.Zero; // Movement
        if (KeyBindings.MoveForward.IsDown()) move += forward;
        if (KeyBindings.MoveBack.IsDown()) move -= forward;
        if (KeyBindings.MoveLeft.IsDown()) move -= right;
        if (KeyBindings.MoveRight.IsDown()) move += right;

        if (KeyBindings.Jump.IsJustPressed()) // Fly check
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
        
        if (KeyBindings.Jump.IsDown() && Player.IsGrounded && !Player.IsFlying) // Jump
        {
            var vel = Player.Velocity;
            vel.Y = JumpForce;
            Player.Velocity = vel;
        }

        if (Player.IsFlying) // Flying
        {
            var vel = Player.Velocity;
            vel.Y = 0;
            if (KeyBindings.Jump.IsDown()) vel.Y = Camera.Speed * 2f;
            if (KeyBindings.Sneak.IsDown()) vel.Y = -Camera.Speed * 2f;
            Player.Velocity = vel;
        }
        
        float targetFov = (float)UserSettings.FOV; // Camera FOV
        if (move != Vector3.Zero) // Check if player is moving
        {
            var vel = Player.Velocity;
            var dir = Vector3D.Normalize(move);
            
            float currentSpeed = Player.IsFlying ? Camera.Speed * 2f : Camera.Speed;
            if (KeyBindings.Sprint.IsDown() && KeyBindings.MoveForward.IsDown()) // Sprinting
            {
                currentSpeed *= 1.3f;
                targetFov = (float)UserSettings.FOV + 20f;
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
            Camera.Fov += ((float)UserSettings.FOV - Camera.Fov) * 10f * (float)Time.DeltaTime;
        }
        
        float lerpSpeed = 10f; 
        Camera.Fov += (targetFov - Camera.Fov) * lerpSpeed * (float)Time.DeltaTime;
        
        // Rotation
        Camera.Look(InputManager.MouseDelta);
        
        // Block actions
        var hit = WorldScene.HitBlock;
        if (hit.HasValue)
        {
            if (KeyBindings.Destroy.IsJustPressed()) _blockActionTimer = 0f;
            if (KeyBindings.Place.IsJustPressed()) _blockActionTimer = 0f;

            bool destroy = KeyBindings.Destroy.IsJustPressed() || 
                           (KeyBindings.Destroy.IsDown() && _blockActionTimer <= 0f);
            bool place = KeyBindings.Place.IsJustPressed() || 
                         (KeyBindings.Place.IsDown() && _blockActionTimer <= 0f);

            if (destroy)
            {
                WorldScene.GameWorld.RemoveBlock(hit.Value.model);
                _blockActionTimer = BlockActionDelay;
            }
            else if (place)
            {
                var pos = new Vector3(hit.Value.model.M41, hit.Value.model.M42, hit.Value.model.M43);
                var newPos = pos + hit.Value.normal;
                var blockAABB = WorldScene.GameWorld.GetBlockAABB(Matrix4X4.CreateTranslation<float>(newPos.X, newPos.Y, newPos.Z));
                if (!blockAABB.Intersects(Player.GetAABB()))
                    WorldScene.GameWorld.AddBlock(newPos.X, newPos.Y, newPos.Z, WorldScene._dirtBlock);
                _blockActionTimer = BlockActionDelay;
            }
            
            if (_blockActionTimer > 0)
                _blockActionTimer -= (float)Time.DeltaTime;
        }
    }
}