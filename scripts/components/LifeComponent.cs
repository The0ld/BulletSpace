using Godot;
using System;

public partial class LifeComponent : Node
{
    [Export]
    public int MaxLives { get; set; } = 3;

    [Export]
    public Node HealthComponentProp { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayerProp { get; set; }

    private int currentLives;

    public int CurrentLives
    {
        get => currentLives;
        set
        {
            currentLives = Mathf.Clamp(value, 0, MaxLives);
            GetNode<GameEvents>("/root/GameEvents").EmitPlayerLifeChange(currentLives);
        }
    }

    public override void _Ready() => CurrentLives = MaxLives;

    public void LostLife() => CurrentLives--;

    public void AddLife() => CurrentLives++;
    
    public bool ShouldReSpawn()
    {
        if (CurrentLives > 0)
            return true;
        else
            return false;
    }

    public void ReSpawn()
    {
        CharacterBody2D parent = GetParent() as CharacterBody2D; 

        string parentGroup = parent.GetGroups()[0];

        parent.GlobalPosition = (GetTree().GetFirstNodeInGroup($"respawn_{parentGroup}") as Node2D).GlobalPosition;

        AnimationPlayerProp.Play("respawn");

        (HealthComponentProp as HealthComponent).ResetHeal();
    }
}
