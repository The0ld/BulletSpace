using Godot;
using System;
using System.Linq;
using OnReadyCs;
using Godot.Collections;

public partial class Player : CharacterBody2D, ICharacter
{
	[OnReady("/root/GameEvents")]
	public GameEvents GameEventsProp { get; set; }

	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; set; }

	[OnReady("/root/MovementAxis")]
	public MovementAxis MovementAxisProp { get; set; }

	[OnReady("MovementComponent")]
	public MovementComponent MovementComponentProp { get; set; }

	[OnReady("LifeComponent")]
	public LifeComponent LifeComponentProp { get; set; }

	[OnReady("%AnimationPlayer")]
	public AnimationPlayer AnimationPlayerProp { get; set; }

	[OnReady("HealthComponent")]
	public Node HealthComponentProp { get; set; }

	[OnReady("ShieldComponent")]
	public Node ShieldComponentProp { get; set; }

	[OnReady("EnergyShieldComponent")]
	public Node2D EnergyShieldComponentProp { get; set; }

	[OnReady("GunComponent")]
	public Marker2D GunComponentProp { get; set; }

	public bool IsDead { get; set; } = false;

	public Vector2 CurrentScale { get; private set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		CurrentScale = Scale;

		var activatedShoot = MetaProgressionProp.GetShootMetaUpgradeActivated();
		(GunComponentProp as GunComponent).PrimaryShoot = activatedShoot.Shoot;
		(GunComponentProp as GunComponent).SetPrimaryShoot();

		(HealthComponentProp as HealthComponent).Died += OnDied;
		(HealthComponentProp as HealthComponent).HealthChanged += OnHealthChanged;
		(ShieldComponentProp as ShieldComponent).ShieldChanged += OnShieldChanged;
		(ShieldComponentProp as ShieldComponent).Deactivate += OnShieldDeactivate;
		(ShieldComponentProp as ShieldComponent).Activate += OnShieldActivate;
		GameEventsProp.CollectDroppedObject += OnCollectDroppedObject;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit += OnHit;

		UpdateHealthPercent();
		UpdateShieldPercent();
		CheckUpgrades();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsDead) return;
		Animations();
		Move();
		ChangeShoot();
	}

	private void ChangeShoot()
	{
		if (Input.IsActionJustPressed("change_shoot"))
		{
			(GunComponentProp as GunComponent).ChangeCurrentShoot();
		}
	}

	private void Animations()
	{
		if (MovementAxisProp.Axis.Y < 0) AnimationPlayerProp.Play("up");
		else if(MovementAxisProp.Axis.Y > 0) AnimationPlayerProp.Play("down");
		else AnimationPlayerProp.Play("idle");
	}

	private void Move()
	{
		MovementComponentProp.AccelerateInDirection(MovementAxisProp.Axis);
		MovementComponentProp.Move(this);
	}

	public void ReSpawn()
	{
		IsDead = false;
		Show();
	}

	private void UpdateHealthPercent()
	{
		float healthPercent = (HealthComponentProp as HealthComponent).GetHealthPercent();
		GameEventsProp.EmitHealthPercentChanged(healthPercent);
	}

	private void UpdateShieldPercent()
	{
		float shieldPercent = (ShieldComponentProp as ShieldComponent).GetShieldPercent();
		GameEventsProp.EmitShieldPercentChanged(shieldPercent);
	}

	private void CheckUpgrades()
	{
		if (!MetaProgressionProp.SaveData.ContainsKey("meta_upgrades")) return;

		var metaUpgrades = (Array<PurchasablePowerupsBasic>)MetaProgressionProp.SaveData["meta_upgrades"];
		
		if (metaUpgrades.Any( u => u.Id == "energy_shield"))
			(ShieldComponentProp as ShieldComponent).FullHeal();
	}

	private void OnHealthChanged() => UpdateHealthPercent();

	private void OnShieldChanged() => UpdateShieldPercent();

	private void OnHit() => GetNode<AudioStreamPlayer2D>("%Hit").Play();

	private void OnShieldDeactivate() => EnergyShieldComponentProp.Visible = false;

	private void OnShieldActivate() => EnergyShieldComponentProp.Visible = true;

	private void OnCollectDroppedObject(Resource resource)
	{
		var droppedResource = resource as StandardDropped;
		if (droppedResource == null) return;

		if (droppedResource.Id == "shield_reload")
			(ShieldComponentProp as ShieldComponent).FullHeal();
		
		if (droppedResource.Id == "health_reload")
			(HealthComponentProp as HealthComponent).Heal(25);

		if (droppedResource.Id == "secondary_shoot")
			(GunComponentProp as GunComponent).SecondaryShoot = (droppedResource as ShootDropped).ShootResource as ShootResource;
	}

	private async void OnDied()
	{
		IsDead = true;
		AnimationPlayerProp.Play("death");

		LifeComponentProp.LostLife();

		bool respawn = LifeComponentProp.ShouldReSpawn();

		await ToSignal(AnimationPlayerProp, "animation_finished");

		if (respawn)
			LifeComponentProp.ReSpawn();
		else
			GameEventsProp.EmitGameOver();
	}

    public override void _ExitTree()
    {
        (HealthComponentProp as HealthComponent).Died -= OnDied;
		(HealthComponentProp as HealthComponent).HealthChanged -= OnHealthChanged;
		(ShieldComponentProp as ShieldComponent).ShieldChanged -= OnShieldChanged;
		(ShieldComponentProp as ShieldComponent).Deactivate -= OnShieldDeactivate;
		(ShieldComponentProp as ShieldComponent).Activate -= OnShieldActivate;
		GameEventsProp.CollectDroppedObject -= OnCollectDroppedObject;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit -= OnHit;
    }
}
