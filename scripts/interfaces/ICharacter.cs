using Godot;

public interface ICharacter
{
    MovementComponent MovementComponentProp { get; set; }
    AnimationPlayer AnimationPlayerProp { get; set; }
    bool IsDead { get; set; }
    Vector2 CurrentScale { get; }
}