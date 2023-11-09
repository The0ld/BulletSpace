using Godot;
using Godot.Collections;
using System;
using System.Linq;
using OnReadyCs;

public partial class BasicEnemy : CharacterBody2D, IEnemy
{
	[Export]
	public int ScoreGain { get; private set; }

	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; set; }

	[OnReady("MovementComponent")]
	public MovementComponent MovementComponentProp { get; set; }

	[OnReady("AnimationPlayer")]
	public AnimationPlayer AnimationPlayerProp { get; set; }

	[OnReady("DropComponent")]
    public DropComponent DropComponentProp { get; set; }

	public bool IsDead { get; set; } = false;

	public Vector2 CurrentScale { get; private set; }

	private PackedScene CoinScene { get; set; } = GD.Load<PackedScene>("res://scenes/game_objects/drops/coin/coin_drop.tscn");

	private PackedScene HealthScene { get; set; } = GD.Load<PackedScene>("res://scenes/game_objects/drops/health/health_drop.tscn");

	private PackedScene ShieldScene { get; set; } = GD.Load<PackedScene>("res://scenes/game_objects/drops/shield/shield_drop.tscn");

	private PackedScene MegaShootScene { get; set; } = GD.Load<PackedScene>("res://scenes/game_objects/drops/mega_shoot/mega_shoot_drop.tscn");

    public override void _Ready()
	{
		this.InitializeOnReadyFields();

		CurrentScale = Scale;

		GetNode<HealthComponent>("HealthComponent").Died += OnDied;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit += OnHit;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited += OnScreenExited;

		DropComponentProp.AddDrop("coin", CoinScene, 25);
		DropComponentProp.AddDrop("health", HealthScene, 30);
		DropComponentProp.AddDrop("mega_shoot", MegaShootScene, 10);

		CheckUpgrades();
	}

	public override void _PhysicsProcess(double delta)
	{
		MovementComponentProp.AccelerateInDirection(new Vector2(CurrentScale.X, 0));
		MovementComponentProp.Move(this);
	}

	private void CheckUpgrades()
	{
		if (!MetaProgressionProp.SaveData.ContainsKey("meta_upgrades")) return;

		var metaUpgrades = (Array<PurchasablePowerupsBasic>)MetaProgressionProp.SaveData["meta_upgrades"];
		
		if (metaUpgrades.Any( u => u.Id == "energy_shield"))
			DropComponentProp.AddDrop("shield", ShieldScene, 30);
	}

	private void OnHit() => GetNode<AudioStreamPlayer2D>("%Hit").Play();

	private void OnScreenExited() => QueueFree();

	private void OnDied()
	{
		IsDead = true;
		GetNode<GameEvents>("/root/GameEvents").EmitScoreIncreased(ScoreGain);
		AnimationPlayerProp.Play("death");
	}

    public override void _ExitTree()
    {
        GetNode<HealthComponent>("HealthComponent").Died -= OnDied;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit -= OnHit;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited -= OnScreenExited;
    }
}
