using Godot;
using System;

public partial class HitBoxComponent : Area2D
{
	[Signal]
	public delegate void ImpactEventHandler();

	[Export]
	public float Damage { get; set; } = 0;

	[Export]
	public bool SusceptibleToImpact { get; set; } = false;

	public override void _Ready() => AreaEntered += OnAreaEntered;

	private void OnAreaEntered(Area2D otherArea)
	{
		if (!SusceptibleToImpact) return;
		if (otherArea is not HurtBoxComponent) return;

		EmitSignal(nameof(Impact));
	}

	public override void _ExitTree() => AreaEntered -= OnAreaEntered;
}
