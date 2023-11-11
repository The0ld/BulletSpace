using Godot;
using System;

public partial class CoinDrop : Node2D, ICollectible
{
	[Export]
	public Resource CollectibleResource { get; set; }

    public override void _Ready() => GetNode<Area2D>("Area2D").AreaEntered += OnAreaEntered;

	public void TweenCollectWithStartPosition(float percent) => TweenCollect(percent, GlobalPosition);

	public void DisableCollision() => GetNode<CollisionShape2D>("%CollisionShape2D").Disabled = true;

	public void Collect()
	{
		GetNode<GameEvents>("/root/GameEvents").EmitCreditAdded(1);
		QueueFree();
	}


	public void TweenCollect(float percent, Vector2 startPosition)
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null) return;

		GlobalPosition = startPosition.Lerp(player.GlobalPosition, percent);
	}

	private void OnAreaEntered(Area2D otherArea)
	{
		this.CallDeferred("DisableCollision");

		var sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
		var energyShield = GetNode<Node2D>("EnergyShieldComponent");

		Tween tween = CreateTween();
		tween.SetParallel();
		tween.TweenMethod(new Callable(this, "TweenCollectWithStartPosition"), 0.0f, 0.7f, 0.3f)
			.SetEase(Tween.EaseType.In)
			.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(sprite, "scale", Vector2.Zero, 0.25f).SetDelay(0.35f);
		tween.TweenProperty(energyShield, "scale", Vector2.Zero, 0.25f).SetDelay(0.35f);
		tween.Chain();

		tween.TweenCallback(new Callable(this, "Collect"));
	}

	public override void _ExitTree() => GetNode<Area2D>("Area2D").AreaEntered -= OnAreaEntered;
}
