using Godot;
using System;

public partial class BulletInfo : Resource
{
    public string Id { get; set; }
    public ShootResource Shoot { get; set; }
    public bool Active { get; set; }
}
