using Godot;
using System;

public partial class HealthShieldManager : Node, IDamageable
{
	[Export]
	public HealthComponent HealthComponentProp { get; set; }

	[Export]
	public ShieldComponent ShieldComponentProp { get; set; }

    public void Damage(float damage)
    {
        bool isShieldActive = ShieldComponentProp.CurrentShield > 0;

        if (isShieldActive)
        {
            ShieldComponentProp.Damage(damage);
            return;
        }

        HealthComponentProp.Damage(damage);
    }
}
