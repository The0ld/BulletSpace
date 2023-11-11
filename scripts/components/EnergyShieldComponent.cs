using Godot;
using System;
using OnReadyCs;

public partial class EnergyShieldComponent : Node2D
{
	[Export]
	public Texture2D BaseTexture { get; set; }

	[OnReady("EnergyShield")]
	public Sprite2D EnergyShield { get; set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();
		
		UpdateTexture(BaseTexture);
	}

	public void UpdateTexture(Texture2D currentTexture) => EnergyShield.Texture = currentTexture;
}
