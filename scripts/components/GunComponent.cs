using Godot;
using Godot.Collections;
using System;
using OnReadyCs;

public partial class GunComponent : Marker2D
{
	private ShootResource _primaryShoot;
	private ShootResource _secondaryShoot;
	private int _secondaryShootAmmo;
	private ShootResource _currentShoot;

	[Export]
	public bool IsPlayer { get; set; }

	[Export]
	public ShootResource PrimaryShoot
	{
		get => _primaryShoot;
		set
		{
			_primaryShoot = value;

			if (_primaryShoot != null && IsPlayer)
				GameEventProp.EmitSetGunShoots("PrimaryShoot", _primaryShoot, true);
		}
	}

	[Export]
	public ShootResource SecondaryShoot
	{
		get => _secondaryShoot;
		set
		{
			if (_secondaryShoot != null && value.Id == _secondaryShoot.Id)
			{
				_secondaryShoot.AmmoCount = _secondaryShootAmmo;
				SetSecondaryShoot();
				return;
			}

			_secondaryShoot = value;
			_secondaryShootAmmo = _secondaryShoot.AmmoCount;

			if (_secondaryShoot != null && IsPlayer)
				GameEventProp.EmitSetGunShoots("SecondaryShoot", _secondaryShoot, false);
		}
	}

	[Export]
	public Node HealthComponentProp { get; set; }

	[OnReady("/root/GameEvents")]
	public GameEvents GameEventProp { get; set; }

	[OnReady("CadenceTimer")]
	public Timer CadenceTimer { get; set; }

	[OnReady("ChangeShootTimer")]
	public Timer ChangeShootTimer { get; set; }

	public ShootResource CurrentShoot
	{
		get => _currentShoot;
		private set
		{
			_currentShoot = value;
			
			if (_currentShoot != null && IsPlayer)
				GameEventProp.EmitSetGunShoots("currentShoot", _currentShoot, true);
		}
	}

	public bool CanShoot { get; set; } = true;

	public bool ParentIsDead { get; set; } = false;

	public bool CanChange { get; private set; } = true;

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		CurrentShoot = PrimaryShoot;
		
		CadenceTimer.Timeout += OnTimeOut;
		ChangeShootTimer.Timeout += OnChangeShootTimerTimeout;
		(HealthComponentProp as HealthComponent).Died += OnDied;
		(HealthComponentProp as HealthComponent).ReSpawned += OnReSpawned;
	}

	public override void _Process(double delta) => CheckCanFire();

	public void SetPrimaryShoot() => CurrentShoot = PrimaryShoot;

	public void SetSecondaryShoot() => CurrentShoot = SecondaryShoot;

	public void ChangeCurrentShoot()
	{
		if (!CanChange) return;

		if (CurrentShoot.Id == PrimaryShoot.Id && SecondaryShoot != null)
			SetSecondaryShoot();
		else
			SetPrimaryShoot();

		CanChange = false;
		ChangeShootTimer.Start();
	}

	private void CheckCanFire()
	{
		if (!CanShoot) return;
        if (CurrentShoot == null) return;

		if (
			(CurrentShoot.IsAutomatic && Input.IsActionPressed("attack") && IsPlayer) ||
            (!CurrentShoot.IsAutomatic && Input.IsActionJustPressed("attack") && IsPlayer) ||
			!IsPlayer
		)
        {
            Fire();
        }
	}

	public void Fire()
    {
        if (CurrentShoot == null) return;
		if (CurrentShoot.AmmoCount == 0) return;
		if (ParentIsDead) return;

		PlayAnimationShoot();
        InstantiateShoot();
        StartCadenceTimer();
		SubtractAmmo();
    }

	private void StartCadenceTimer()
	{
		CanShoot = false;
        CadenceTimer.WaitTime = CurrentShoot.FireRate;
        CadenceTimer.Start();
	}

	private void PlayAnimationShoot()
	{
		var flashAnimation = GetNode<AnimatedSprite2D>("FlashShot");
		flashAnimation.Animation = "flash";
		flashAnimation.Play();
	}

	private void InstantiateShoot()
	{
		var currentShootInstance = CurrentShoot.ShootScene.Instantiate();
		
		(currentShootInstance as Node2D).GlobalPosition = GlobalPosition;
		(currentShootInstance as Node2D).Scale = GetParent<ICharacter>().CurrentScale;
		(currentShootInstance as IShoot).IsFriend = IsPlayer;
		(currentShootInstance as IShoot).Speed *= GetParent<ICharacter>().CurrentScale.X;

		GetTree().GetFirstNodeInGroup("bullets").AddChild(currentShootInstance);
	}

	private void SubtractAmmo()
	{
		if (CurrentShoot.AmmoCount > 0)
		{
			CurrentShoot.AmmoCount -= 1;

			if (CurrentShoot.Id == PrimaryShoot.Id)
			{
				PrimaryShoot.AmmoCount = CurrentShoot.AmmoCount;
				GameEventProp.EmitSetGunShoots("PrimaryShoot", _primaryShoot, true);
			}
			else if (CurrentShoot.Id == SecondaryShoot.Id)
			{
				SecondaryShoot.AmmoCount = CurrentShoot.AmmoCount;
				GameEventProp.EmitSetGunShoots("SecondaryShoot", _secondaryShoot, true);
			}
		}
	}

	private void OnChangeShootTimerTimeout() => CanChange = true;

	private void OnTimeOut() => CanShoot = true;

	private void OnDied() => ParentIsDead = true;

	private void OnReSpawned() => ParentIsDead = false;

    public override void _ExitTree()
    {
        CadenceTimer.Timeout -= OnTimeOut;
		ChangeShootTimer.Timeout -= OnChangeShootTimerTimeout;
		(HealthComponentProp as HealthComponent).Died -= OnDied;
		(HealthComponentProp as HealthComponent).ReSpawned -= OnReSpawned;
    }
}
