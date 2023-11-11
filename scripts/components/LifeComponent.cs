using Godot;
using System;

public partial class LifeComponent : Node
{
    [Export]
    public int MaxLives { get; set; } = 3;

    [Export]
    public HealthComponent HealthComponentProp { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayerProp { get; set; }

    private int _currentLives;

    public int CurrentLives
    {
        get => _currentLives;
        set
        {
            _currentLives = Mathf.Clamp(value, 0, MaxLives);
            GetNode<GameEvents>("/root/GameEvents").EmitPlayerLifeChange(_currentLives);
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

        HealthComponentProp.ResetHeal();
    }
}
