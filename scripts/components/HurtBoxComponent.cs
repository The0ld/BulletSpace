using Godot;
using OnReadyCs;
using System;

public partial class HurtBoxComponent : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public double InvulnerabilityFrames { get; set; } = 1;

	[Export]
	public Node HealthOrShieldComponentProp { get; set; }

	[OnReady("InvulnerabilityTimer")]
	public Timer InvulnerabilityTimer { get; set; }

	public bool IsVulnerable { get; private set; } = true;

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		InvulnerabilityTimer.WaitTime = InvulnerabilityFrames;

		AreaEntered += OnAreaEntered;
		InvulnerabilityTimer.Timeout += OnInvulnerabilityTimerTimeOut;
	}

	private void StartInvulnerability()
	{
		IsVulnerable = false;
		InvulnerabilityTimer.Start();
	}

	private void OnInvulnerabilityTimerTimeOut() => IsVulnerable = true;
	
	private void OnAreaEntered(Area2D otherArea)
	{

		if (otherArea is not HitBoxComponent hitBoxComponent) return;
		if (HealthOrShieldComponentProp == null) return;
		if (!IsVulnerable) return;

		(HealthOrShieldComponentProp as IDamageable).Damage(hitBoxComponent.Damage);

		EmitSignal(nameof(Hit));
		StartInvulnerability();
	}

    public override void _ExitTree()
    {
        AreaEntered -= OnAreaEntered;
		InvulnerabilityTimer.Timeout -= OnInvulnerabilityTimerTimeOut;
    }
}
