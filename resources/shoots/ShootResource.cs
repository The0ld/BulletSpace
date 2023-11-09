using Godot;
using System;

public partial class ShootResource : Resource
{
    [Export]
    public string Id { get; set; }

    [Export]
    public PackedScene ShootScene { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public float FireRate { get; set; } = 1.0f;

    [Export]
    public int AmmoCount { get; set; } = -1;

    [Export]
    public bool IsAutomatic { get; set; } = false;
}
