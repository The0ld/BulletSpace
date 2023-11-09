using Godot;
using System;

public partial class PurchasablePowerupsShoots : PurchasablePowerupsBasic
{
    [Export]
    public bool Activated { get; set; }

    [Export]
    public ShootResource Shoot { get; set; }
}
