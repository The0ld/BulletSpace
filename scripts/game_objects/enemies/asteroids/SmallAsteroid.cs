using Godot;
using System;
using OnReadyCs;

public partial class SmallAsteroid : CharacterBody2D, IEnemy
{
	[Export]
	public int ScoreGain { get; private set; }

	[OnReady("MovementComponent")]
    public MovementComponent MovementComponentProp { get; set; }

	[OnReady("AnimationPlayer")]
    public AnimationPlayer AnimationPlayerProp { get; set; }

	[OnReady("DropComponent")]
    public DropComponent DropComponentProp { get; set; }

    public bool IsDead { get; set; }

    public Vector2 CurrentScale { get; private set; }

	private PackedScene CoinScene { get; set; } = GD.Load<PackedScene>("res://scenes/game_objects/drops/coin/coin_drop.tscn");

    public override void _Ready()
	{
		this.InitializeOnReadyFields();

		CurrentScale = Scale;

		GetNode<HealthComponent>("HealthComponent").Died += OnDied;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit += OnHit;
		GetNode<HitBoxComponent>("HitBoxComponent").Impact += OnImpact;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited += OnScreenExited;

		DropComponentProp.AddDrop("coin", CoinScene, 5);
	}

	public override void _PhysicsProcess(double delta)
	{
		MovementComponentProp.AccelerateInDirection(new Vector2(CurrentScale.X, 0));
		MovementComponentProp.Move(this);
	}

	private void Die()
	{
		IsDead = true;
		GetNode<GameEvents>("/root/GameEvents").EmitScoreIncreased(ScoreGain);
		AnimationPlayerProp.Play("death");
	}

	private void OnDied() => Die();

	private void OnImpact() => Die();

	private void OnHit() => GetNode<AudioStreamPlayer2D>("%Hit").Play();

	private void OnScreenExited() => QueueFree();

	public override void _ExitTree()
    {
        GetNode<HealthComponent>("HealthComponent").Died -= OnDied;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit -= OnHit;
		GetNode<HitBoxComponent>("HitBoxComponent").Impact -= OnImpact;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited -= OnScreenExited;
    }
}
