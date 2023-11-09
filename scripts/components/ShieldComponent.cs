using Godot;
using System;

public partial class ShieldComponent : Node, IDamageable
{
    [Signal]
	public delegate void DeactivateEventHandler();

    [Signal]
	public delegate void ActivateEventHandler();

    [Signal]
	public delegate void ShieldChangedEventHandler();

	[Signal]
	public delegate void ShieldDecreasedEventHandler();

    [Export]
	public float MaxShield { get; set; } = 0;

    public float CurrentShield { get; private set; }

    public override void _Ready() => CurrentShield = 0;

    public void Heal(int healAmount) => Damage(-healAmount);

    public void FullHeal() => Damage(-MaxShield);
    
    public void Damage(float damage)
    {
        if (damage < 0 && CurrentShield <= 0) EmitSignal(nameof(Activate));

        CurrentShield = Math.Clamp(CurrentShield - damage, 0, MaxShield);
        EmitSignal(nameof(ShieldChanged));

        if (damage > 0) EmitSignal(nameof(ShieldDecreased));

        this.CallDeferred("CheckDeactivate");
    }

    public float GetShieldPercent()
    {
        if (MaxShield <= 0) return 0;

        return Math.Min(CurrentShield / MaxShield, 1);
    }

    private void CheckDeactivate()
	{
		if (CurrentShield == 0) EmitSignal(nameof(Deactivate));
	}
}
