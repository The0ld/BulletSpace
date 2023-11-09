using Godot;
using System;
using OnReadyCs;

public partial class PowerupCard : Control
{
	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; set; }

	[OnReady("/root/GameEvents")]
	public GameEvents GameEventsProp { get; set; }

	[OnReady("%Title")]
	public Label Title { get; set; }

	[OnReady("%Icon")]
	public TextureRect Icon { get; set; }

	[OnReady("%Description")]
	public Label Description { get; set; }

	[OnReady("%Price")]
	public Label Price { get; set; }

	[OnReady("%Button")]
	public Button Button { get; set; }

	[OnReady("%Control")]
	public Control ControlProp { get; set; }

	private int AvailableCredits { get; set; }

	private PurchasablePowerupsBasic CurrentPowerUp { get; set; }
	
	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		Button.Pressed += OnPressed;
		MetaProgressionProp.ChangeGlobalStatus += OnChangeGlobalStatus;
	}

	private void SetAvailableCredits() => AvailableCredits = MetaProgressionProp.GetCredit();

	public void SetDataToCard(PurchasablePowerupsBasic item)
	{
		CurrentPowerUp = item;
		Title.Text = item.Name;
		Icon.Texture = item.Icon;
		Description.Text = item.Description;

		UpdateProgress();
	}

	private void UpdateProgress()
	{
		SetAvailableCredits();
		
		if (Price.Visible)
			Price.Text = $"{Math.Min(AvailableCredits, CurrentPowerUp.Price)}/{CurrentPowerUp.Price}";
		
		bool bought = MetaProgressionProp.CheckUpgradeAdded(CurrentPowerUp.Id);

		if (bought && CurrentPowerUp is PurchasablePowerupsShoots shootsPowerUp)
		{
			Button.Text = shootsPowerUp.Activated ? "ACTIVATED" : "ACTIVATE";
        	Button.Disabled = shootsPowerUp.Activated;
			Price.Visible = false;
			ControlProp.Visible = false;
		}
		else
		{
			Button.Disabled = Math.Min(AvailableCredits, CurrentPowerUp.Price) < CurrentPowerUp.Price || bought;

			if (bought)
			{
				Price.Visible = false;
				ControlProp.Visible = false;
				Button.Text = "BOUGHT";
			}
		}
	}

	private void OnChangeGlobalStatus() => UpdateProgress();

	private void OnPressed()
	{
		bool bought = MetaProgressionProp.CheckUpgradeAdded(CurrentPowerUp.Id);

		if (bought && CurrentPowerUp is PurchasablePowerupsShoots shootsPowerUp)
			MetaProgressionProp.ActiveShoot(shootsPowerUp.Id);
		else
		{
			MetaProgressionProp.AddMetaUpgrade(CurrentPowerUp);
			GameEventsProp.EmitCreditAdded(-CurrentPowerUp.Price);
		}

		UpdateProgress();
		MetaProgressionProp.Save();
	}

    public override void _ExitTree()
    {
        Button.Pressed -= OnPressed;
		MetaProgressionProp.ChangeGlobalStatus -= OnChangeGlobalStatus;
    }
}
