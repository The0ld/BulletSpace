using Godot;
using System;

public partial class PurchasablePowerupsBasic : Resource
{
    [Export]
    public string Id { get; set; }

    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public int Price { get; set; }
}
