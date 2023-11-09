using Godot;
using Godot.Collections;
using System;
using System.Linq;
using OnReadyCs;

public partial class Gui : Control
{
	[Export]
	public Node ScoreManagerProp { get; set; }

	[Export]
	public Node CreditManagerProp { get; set; }

	[Export]
	public Node LifeManagerProp { get; set; }

	[OnReady("/root/GameEvents")]
	public GameEvents GameEventsProp { get; set; }

	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; set; }

	[OnReady("%BarShieldComponent")]
	public ProgressBar BarShieldComponent { get; set; }

	[OnReady("%Score")]
	public Label Score { get; private set; }

	[OnReady("%Credits")]
	public Label Credits { get; private set; }

	[OnReady("%Level")]
	public Label Level { get; private set; }

	[OnReady("%Lives")]
	public Label Lives { get; private set; }

	public Array<BulletInfo> Bullets { get; set; } = new Array<BulletInfo>();

	public override void _EnterTree()
	{
		this.InitializeOnReadyFields();

		Credits.Text = $"CREDIT: 0";
		Score.Text = $"SCORE: 0";
		Lives.Text = $"LIVES: 0";

		(ScoreManagerProp as ScoreManager).ScoreUpdated += OnScoreUpdated;
		(CreditManagerProp as CreditManager).CreditUpdated += OnCreditUpdated;
		(LifeManagerProp as LifeManager).LifeUpdated += OnLifeUpdated;
		GameEventsProp.HealthPercentChanged += OnHealthPercentChanged;
		GameEventsProp.ShieldPercentChanged += OnShieldPercentChanged;
		GameEventsProp.SetGunShoots += OnSetGunShoots;

		CheckUpgrades();
	}

	private void RefreshBullets()
	{
		foreach (var bullet in Bullets)
		{
			var ammoAmount = bullet.Shoot.AmmoCount > -1 ? bullet.Shoot.AmmoCount : 99;
			var textureRect = GetNode<TextureRect>($"%{bullet.Id}/SpriteShoot");
			var ammoAmountLabel = GetNode<Label>($"%{bullet.Id}/AmmoAmount");

			textureRect.Texture = bullet.Shoot.Texture;
			ammoAmountLabel.Text = $"x{ammoAmount}";

			ChangeBulletState(textureRect, ammoAmountLabel, bullet.Active);
		}
	}

	private void ChangeBulletState(TextureRect textureRect, Label ammoAmountLabel, bool active)
	{
		if (active)
		{	
			(textureRect.Material as ShaderMaterial).SetShaderParameter("lerp_percent", 0);
			ammoAmountLabel.AddThemeColorOverride("font_color", new Color(0xfc / 255.0f, 0xbe / 255.0f, 0x21 / 255.0f, 1.0f));
			ammoAmountLabel.AddThemeColorOverride("font_shadow_color", new Color(0xba / 255.0f, 0x19 / 255.0f, 0x24 / 255.0f, 1.0f));
		}
		else
		{
			(textureRect.Material as ShaderMaterial).SetShaderParameter("lerp_percent", 0.5);
			ammoAmountLabel.AddThemeColorOverride("font_color", new Color(0xe0 / 255.0f, 0xe0 / 255.0f, 0xe0 / 255.0f, 1.0f));
			ammoAmountLabel.AddThemeColorOverride("font_shadow_color", new Color(0xb0 / 255.0f, 0xb0 / 255.0f, 0xb0 / 255.0f, 1.0f));
		}
	}

    private void CheckUpgrades()
	{
		if (!MetaProgressionProp.SaveData.ContainsKey("meta_upgrades")) return;

		var metaUpgrades = (Array<PurchasablePowerupsBasic>)MetaProgressionProp.SaveData["meta_upgrades"];
		
		if (metaUpgrades.Any( u => u.Id == "energy_shield"))
		{
			BarShieldComponent.Visible = true;
		}
	}

	private void AddOrUpdateBullet(BulletInfo existingBullet, BulletInfo shoot)
	{
		if (existingBullet != null)
			Bullets[Bullets.IndexOf(existingBullet)] = shoot;
		else if (existingBullet == null && shoot.Id != "currentShoot")
			Bullets.Add(shoot);
	}

	private void ActivateAndDeactivateBullet(BulletInfo shoot)
	{
		if (shoot.Id != "currentShoot") return;

		foreach (BulletInfo bullet in Bullets)
		{
			if (bullet.Shoot.Id == shoot.Shoot.Id)
				bullet.Active = true;
			else
				bullet.Active = false;
		}
	}

	public void OnHealthPercentChanged(float percent)
	{
		if (GetNode<ProgressBar>("%BarHealthComponent") is not BarHealthComponent healthBar) return;
		healthBar.SetPercent(percent);
	}

	public void OnShieldPercentChanged(float percent)
	{
		if (BarShieldComponent is not BarShieldComponent shieldBar) return;
		shieldBar.SetPercent(percent);
	}

	public void OnSetGunShoots(BulletInfo shoot)
	{
		var existingBullet = Bullets.FirstOrDefault( b => b.Id == shoot.Id);

		AddOrUpdateBullet(existingBullet, shoot);
		ActivateAndDeactivateBullet(shoot);
		RefreshBullets();
	}

	public void OnScoreUpdated(int score) => Score.Text = $"SCORE: {score}";

	public void OnCreditUpdated(int credit) => Credits.Text = $"CREDITS: {credit}";

	public void OnLifeUpdated(int life) => Lives.Text = $"LIVES: {life}";

	public override void _ExitTree()
	{
		(ScoreManagerProp as ScoreManager).ScoreUpdated -= OnScoreUpdated;
		(CreditManagerProp as CreditManager).CreditUpdated -= OnCreditUpdated;
		(LifeManagerProp as LifeManager).LifeUpdated -= OnLifeUpdated;
		GameEventsProp.HealthPercentChanged -= OnHealthPercentChanged;
		GameEventsProp.ShieldPercentChanged -= OnShieldPercentChanged;
		GameEventsProp.SetGunShoots -= OnSetGunShoots;
	}
}
