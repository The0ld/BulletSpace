using Godot;
using System;

public partial class MovementComponent : Node
{
    [Export]
    public float MaxSpeed { get; set; } = 40f;

    [Export]
    public float Acceleration { get; set; } = 5.0f;

    public Vector2 Velocity { get; private set; } = Vector2.Zero;

    public void DeAccelerate() => AccelerateInDirection(Vector2.Zero);
    
    public void AccelerateInDirection(Vector2 direction)
    {
        var desiredVelocity = direction * MaxSpeed;

        Velocity = Velocity.Lerp(desiredVelocity, (float)(1 - Math.Exp(-Acceleration * GetProcessDeltaTime())));
    }

    public void Move(CharacterBody2D characterBody)
    {
        characterBody.Velocity = Velocity;
        characterBody.MoveAndSlide();

        Velocity = characterBody.Velocity;
    }
}
