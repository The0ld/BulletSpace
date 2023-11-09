using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class MetaProgression : Node
{
	[Signal]
	public delegate void ChangeGlobalStatusEventHandler();

	public string SaveFilePath { get; } = "user://game.save";

	private Array<PurchasablePowerupsBasic> _metaUpgradesBackup;

	public Dictionary SaveData { get; private set; } = new Dictionary {
		{"meta_credits", 0},
		{"score_ranking", new Array<Dictionary> {}},
        {"meta_upgrades", new Array<PurchasablePowerupsBasic>
			{ GD.Load<PurchasablePowerupsBasic>("res://resources/purchasable_powerups/purchasable_basic_shoot.tres") }
		}
    };

    public override void _Ready()
    {
		GetNode<GameEvents>("/root/GameEvents").CreditAdded += OnCreditAdded;
		Load();
    }

	private void Load()
	{
		if (!FileAccess.FileExists(SaveFilePath)) return;

		using var file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read);

		var dataFile = file.GetVar();
		if (dataFile.VariantType != Variant.Type.Dictionary) return;

		SaveData = (Dictionary)dataFile;

		if (!SaveData.ContainsKey("meta_upgrades")) return;

		var resourcePaths = (Array<string>)SaveData["meta_upgrades"];
		var metaUpgrades = new Array<PurchasablePowerupsBasic>();

		foreach (var resourcePath in resourcePaths)
		{
			var powerupResource = GD.Load<PurchasablePowerupsBasic>(resourcePath);
			if (powerupResource == null) continue;
			metaUpgrades.Add(powerupResource);
		}

		SaveData["meta_upgrades"] = metaUpgrades;
	}

	public void Save()
	{
		BackupMetaUpgrades();
		SaveIndividualPowerups();
		SaveAllPowerupPaths();
		RestoreMetaUpgradesBackup();
	}

	private void BackupMetaUpgrades()
	{
		_metaUpgradesBackup = (Array<PurchasablePowerupsBasic>)SaveData["meta_upgrades"];
	}

	private void SaveIndividualPowerups()
	{
		foreach (PurchasablePowerupsBasic powerup in _metaUpgradesBackup)
		{
			TrySavePowerup(powerup);
		}
	}

	private void TrySavePowerup(PurchasablePowerupsBasic powerup)
	{
		if (!string.IsNullOrWhiteSpace(powerup.ResourcePath))
		{
			var saveError = ResourceSaver.Save(powerup, powerup.ResourcePath);
			if (saveError != Error.Ok)
				GD.PrintErr("Failed to save resource at path: ", powerup.ResourcePath);
		}
	}

	private void SaveAllPowerupPaths()
	{
		var resourcePaths = _metaUpgradesBackup.Select(p => p.ResourcePath).ToArray();
		SaveData["meta_upgrades"] = resourcePaths;
		SaveDataToFile();
	}

	private void SaveDataToFile()
	{
		using var file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
		file.StoreVar(SaveData);
	}

	private void RestoreMetaUpgradesBackup()
	{
		SaveData["meta_upgrades"] = _metaUpgradesBackup;
	}

	public void AddMetaUpgrade(PurchasablePowerupsBasic upgrade)
	{
		var metaUpgrades = (Array<PurchasablePowerupsBasic>)SaveData["meta_upgrades"];
		var existingUpgrade = metaUpgrades.Where(u => u.Id == upgrade.Id).ToArray();

		if (existingUpgrade.Length > 0) return;

		metaUpgrades.Add(upgrade);

		EmitSignal(nameof(ChangeGlobalStatus));
		Save();
	}

	public void ActiveShoot(string id)
	{
		var metaUpgrades = (Array<PurchasablePowerupsBasic>)SaveData["meta_upgrades"];
		metaUpgrades.OfType<PurchasablePowerupsShoots>()
                .ToList()
                .ForEach(upgrade => upgrade.Activated = upgrade.Id == id);
		
		EmitSignal(nameof(ChangeGlobalStatus));
		Save();
	}

	public PurchasablePowerupsShoots GetShootMetaUpgradeActivated()
	{
		var metaUpgrades = (Array<PurchasablePowerupsBasic>)SaveData["meta_upgrades"];

		var shootActivated = metaUpgrades
			.OfType<PurchasablePowerupsShoots>()
			.FirstOrDefault(shoot => shoot.Activated);

		if (shootActivated == null)
			shootActivated = metaUpgrades
				.OfType<PurchasablePowerupsShoots>()
				.FirstOrDefault();

		return shootActivated;
	}

	public bool CheckUpgradeAdded(string id)
	{
		var metaUpgrades = (Array<PurchasablePowerupsBasic>)SaveData["meta_upgrades"];
		return metaUpgrades.Any( u => u.Id == id);
	}

	public void SaveScore(string name, int score)
	{
		Array<Dictionary> scoreRanking = (Array<Dictionary>)SaveData["score_ranking"];

		#nullable enable
		Dictionary? foundScore = scoreRanking.FirstOrDefault(score => (string)score["name"] == name);
		if (foundScore == null)
			scoreRanking.Add(
				new Dictionary()
				{
					{"name", name},
					{"score", score}
				}
			);
		else
			foundScore["score"] = score;
	}

	public Dictionary[]? GetScoreRanking()
	{
		Array<Dictionary> scoreRanking = (Array<Dictionary>)SaveData["score_ranking"];

		var rankingOrdered = scoreRanking.OrderByDescending(score => (int)score["score"]).ToArray();

		return rankingOrdered;
	}

	public int GetCredit() => (int)SaveData["meta_credits"];

	public void OnCreditAdded(int credit)
	{
		int currentCredits = (int)SaveData["meta_credits"];
		currentCredits += credit;
		SaveData["meta_credits"] = currentCredits;
	}

    public override void _ExitTree() => GetNode<GameEvents>("/root/GameEvents").CreditAdded -= OnCreditAdded;
}
