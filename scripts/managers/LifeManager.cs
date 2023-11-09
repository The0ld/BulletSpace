using Godot;
using System;

public partial class LifeManager : Node
{
    [Signal]
    public delegate void LifeUpdatedEventHandler(int life);

    public override void _Ready() => GetNode<GameEvents>("/root/GameEvents").PlayerLifeChange += OnPlayerLifeChange;

    private void OnPlayerLifeChange(int life) => EmitSignal(nameof(LifeUpdated), life);
    
    public override void _ExitTree() => GetNode<GameEvents>("/root/GameEvents").PlayerLifeChange -= OnPlayerLifeChange;
}
