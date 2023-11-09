using Godot;
using System;

public partial class StandardDropped : Resource
{
    [Export]
	public string Id { get; set; }

	[Export]
	public int MaxQuantity { get; set; }

	[Export]
	public string Name { get; set; }

	[Export(PropertyHint.MultilineText)]
	public string Description { get; set; }
}
