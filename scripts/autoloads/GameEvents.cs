using Godot;
using Godot.Collections;
using System;

public partial class GameEvents : Node
{
    [Signal]
    public delegate void ScoreIncreasedEventHandler(int score);

    [Signal]
    public delegate void CreditAddedEventHandler(int credit);

    [Signal]
    public delegate void PlayerLifeChangeEventHandler(int life);

    [Signal]
    public delegate void PlayerDamageEventHandler();

    [Signal]
    public delegate void GameOverEventHandler();

    [Signal]
    public delegate void CollectDroppedObjectEventHandler(Resource resource);

    [Signal]
    public delegate void HealthPercentChangedEventHandler(float percent);

    [Signal]
    public delegate void ShieldPercentChangedEventHandler(float percent);

    [Signal]
	public delegate void AbilityUpgradeAddedEventHandler(AbilityUpgrade upgrade, Dictionary currentUpgrades);

    [Signal]
	public delegate void SetGunShootsEventHandler(BulletInfo shoot);

    public void EmitScoreIncreased(int score) => EmitSignal(nameof(ScoreIncreased), score);

    public void EmitCreditAdded(int credit) => EmitSignal(nameof(CreditAdded), credit);

    public void EmitPlayerLifeChange(int life) => EmitSignal(nameof(PlayerLifeChange), life);

    public void EmitHealthPercentChanged(float percent) => EmitSignal(nameof(HealthPercentChanged), percent);

    public void EmitShieldPercentChanged(float percent) => EmitSignal(nameof(ShieldPercentChanged), percent);

    public void EmitPlayerDamage() => EmitSignal(nameof(PlayerDamage));

    public void EmitGameOver() => EmitSignal(nameof(GameOver));

    public void EmitCollectDroppedObject(Resource resource) => EmitSignal(nameof(CollectDroppedObject), resource);
    
    public void EmitAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary currentUpgrades) =>
        EmitSignal(nameof(AbilityUpgradeAdded), upgrade, currentUpgrades);

    public void EmitSetGunShoots(string id, ShootResource shootResource, bool active)
    {
        var shootInfo = new BulletInfo()
        {
            Id = id,
            Shoot = shootResource,
            Active = active
        };

        EmitSignal(nameof(SetGunShoots), shootInfo);
    }
}
