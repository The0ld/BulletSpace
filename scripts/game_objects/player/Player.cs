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
	public HealthComponent HealthComponentProp { get; set; }

	[OnReady("ShieldComponent")]
	public ShieldComponent ShieldComponentProp { get; set; }

	[OnReady("EnergyShieldComponent")]
	public EnergyShieldComponent EnergyShieldComponentProp { get; set; }

	[OnReady("GunComponent")]
	public GunComponent GunComponentProp { get; set; }

	public bool IsDead { get; set; } = false;

	public Vector2 CurrentScale { get; private set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		CurrentScale = Scale;

		var activatedShoot = MetaProgressionProp.GetShootMetaUpgradeActivated();
		GunComponentProp.PrimaryShoot = activatedShoot.Shoot;
		GunComponentProp.SetPrimaryShoot();

		HealthComponentProp.Died += OnDied;
		HealthComponentProp.HealthChanged += OnHealthChanged;
		ShieldComponentProp.ShieldChanged += OnShieldChanged;
		ShieldComponentProp.Deactivate += OnShieldDeactivate;
		ShieldComponentProp.Activate += OnShieldActivate;
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
			GunComponentProp.ChangeCurrentShoot();
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
		float healthPercent = HealthComponentProp.GetHealthPercent();
		GameEventsProp.EmitHealthPercentChanged(healthPercent);
	}

	private void UpdateShieldPercent()
	{
		float shieldPercent = ShieldComponentProp.GetShieldPercent();
		GameEventsProp.EmitShieldPercentChanged(shieldPercent);
	}

	private void CheckUpgrades()
	{
		if (!MetaProgressionProp.SaveData.ContainsKey("meta_upgrades")) return;

		var metaUpgrades = (Array<PurchasablePowerupsBasic>)MetaProgressionProp.SaveData["meta_upgrades"];
		
		if (metaUpgrades.Any( u => u.Id == "energy_shield"))
			ShieldComponentProp.FullHeal();
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
			ShieldComponentProp.FullHeal();
		
		if (droppedResource.Id == "health_reload")
			HealthComponentProp.Heal(25);

		if (droppedResource.Id == "secondary_shoot")
			GunComponentProp.SecondaryShoot = (droppedResource as ShootDropped).ShootResource as ShootResource;
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
        HealthComponentProp.Died -= OnDied;
		HealthComponentProp.HealthChanged -= OnHealthChanged;
		ShieldComponentProp.ShieldChanged -= OnShieldChanged;
		ShieldComponentProp.Deactivate -= OnShieldDeactivate;
		ShieldComponentProp.Activate -= OnShieldActivate;
		GameEventsProp.CollectDroppedObject -= OnCollectDroppedObject;
		GetNode<HurtBoxComponent>("HurtBoxComponent").Hit -= OnHit;
    }
}
