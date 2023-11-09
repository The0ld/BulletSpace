using Godot;
using System;
using OnReadyCs;

public partial class BasicShoot : Node2D, IShoot
{
	[Export]
	public float Speed { get; set; } = 296;

	[Export]
	public bool IsFriend { get; set; } = true;

	[OnReady("HitBoxComponent")]
	public Area2D HitBoxComponent { get; private set; }

	public bool Impacted { get; set; } = false;

	public override void _Ready()
	{
		this.InitializeOnReadyFields();
		SetCollisionLayerAndMask();

		(HitBoxComponent as HitBoxComponent).Impact += OnImpact;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited += OnScreenExited;
	}
	
	public override void _Process(double delta) => MoveProjectile((float)delta);

	private void SetCollisionLayerAndMask()
	{
		if (IsFriend)
		{
			HitBoxComponent.SetCollisionLayerValue((int)CollisionLayer.FRIEND, true);
			HitBoxComponent.SetCollisionMaskValue((int)CollisionMask.ENEMY, true);
		}
		else
		{
			HitBoxComponent.SetCollisionLayerValue((int)CollisionLayer.ENEMY, true);
			HitBoxComponent.SetCollisionMaskValue((int)CollisionMask.FRIEND, true);
		}
	}

	public void MoveProjectile(float delta)
	{
		if (Impacted) return;
		GlobalPosition += new Vector2(Speed * (float)delta, 0);
	}

	public void OnScreenExited() => QueueFree();
	
	public void OnImpact()
	{
		Impacted = true;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("hit");
	}

	public override void _ExitTree()
    {
        (HitBoxComponent as HitBoxComponent).Impact -= OnImpact;
		GetNode<VisibleOnScreenNotifier2D>("ScreenNotifier").ScreenExited -= OnScreenExited;
    }
}
