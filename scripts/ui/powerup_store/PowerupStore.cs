using Godot;
using System;
using Godot.Collections;
using OnReadyCs;

public partial class PowerupStore : CanvasLayer
{
	[Export]
	public Array<PurchasablePowerupsBasic> PurchaseItems { get; set; } = new Array<PurchasablePowerupsBasic>();

	[OnReady("%Back")]
	public Button Back { get; private set; }

	public PackedScene PowerupCardProp { get; set; } = GD.Load<PackedScene>("res://scenes/ui/powerup_store/powerup_card.tscn");

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		Back.Pressed += OnBackPressed;
		InstantiatePowerupCards();
	}

	private void InstantiatePowerupCards()
	{
		if (PurchaseItems.Count <= 0) return;

		foreach (PurchasablePowerupsBasic item in PurchaseItems)
		{
			var powerupCardInstance = PowerupCardProp.Instantiate() as PowerupCard;

			GetNode<GridContainer>("%GridContainer").AddChild(powerupCardInstance);

			powerupCardInstance.SetDataToCard(item);
		}
	}

	private void OnBackPressed() => GetTree().ChangeSceneToFile("res://scenes/ui/main_menu/main_menu.tscn");

	public override void _ExitTree() => Back.Pressed -= OnBackPressed;
}
