using Godot;
using System;

public partial class HealthShieldManager : Node, IDamageable
{
	[Export]
	public Node HealthComponentProp { get; set; }

	[Export]
	public Node ShieldComponentProp { get; set; }

    public void Damage(float damage)
    {
        if (HealthComponentProp is not HealthComponent healthComponent) return;
        if (ShieldComponentProp is not ShieldComponent shieldComponent) return;

        bool isShieldActive = shieldComponent.CurrentShield > 0;

        if (isShieldActive)
        {
            shieldComponent.Damage(damage);
            return;
        }

        healthComponent.Damage(damage);
    }
}
