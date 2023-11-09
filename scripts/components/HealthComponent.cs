using Godot;
using System;

public partial class HealthComponent : Node, IDamageable
{
    [Signal]
	public delegate void DiedEventHandler();

    [Signal]
	public delegate void ReSpawnedEventHandler();

    [Signal]
	public delegate void HealthChangedEventHandler();

	[Signal]
	public delegate void HealthDecreasedEventHandler();

    [Export]
	public float MaxHealth { get; set; } = 100;

    public float CurrentHealth { get; private set; }

    public override void _Ready() => CurrentHealth = MaxHealth;

    public void Damage(float damage)
    {
        CurrentHealth = Math.Clamp(CurrentHealth - damage, 0, MaxHealth);
        EmitSignal(nameof(HealthChanged));

        if (damage > 0) EmitSignal(nameof(HealthDecreased));

        this.CallDeferred("CheckDeath");
    }

    public void Heal(float healAmount) => Damage(-healAmount);

    public float GetHealthPercent()
    {
        if (MaxHealth <= 0) return 0;

        return Math.Min(CurrentHealth / MaxHealth, 1);
    }

    public void ResetHeal()
    {
        Heal(MaxHealth);
        EmitSignal(nameof(ReSpawned));
    }

    private void CheckDeath()
	{
		if (CurrentHealth == 0)
			EmitSignal(nameof(Died));
	}
}
