using Godot;
using System;
using OnReadyCs;

public partial class MainMenu : Control
{
	private const string StartButton = "%Start";
	private const string UpgradesButton = "%Upgrades";
	private const string OptionsButton = "%Options";
	private const string QuitButton = "%Quit";
	private const string RankingButton = "%Ranking";

	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; set; }

	public PackedScene OptionScene { get; private set; } = GD.Load<PackedScene>("res://scenes/ui/options_menu/options_menu.tscn");

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		SubscribeToButtonEvents();
	}

	private void SubscribeToButtonEvents()
	{
		GetNode<Button>(StartButton).Pressed += OnStartPressed;
		GetNode<Button>(UpgradesButton).Pressed += OnUpgradesPressed;
		GetNode<Button>(OptionsButton).Pressed += OnOptionsPressed;
		GetNode<Button>(QuitButton).Pressed += OnQuitPressed;
		GetNode<Button>(RankingButton).Pressed += OnRankingPressed;
	}

	private void OnOptionsPressed()
	{
		var optionInstance = OptionScene.Instantiate();
		AddChild(optionInstance);

		(optionInstance as OptionsMenu).BackPressed += () => OnBackPressed(optionInstance);
	}

	private void OnStartPressed() => GetTree().ChangeSceneToFile("res://scenes/levels/level_one.tscn");

	private void OnUpgradesPressed() => GetTree().ChangeSceneToFile("res://scenes/ui/powerup_store/powerup_store.tscn");

	private void OnBackPressed(Node optionsInstance) => optionsInstance.QueueFree();

	private void OnQuitPressed() => GetTree().Quit();

	private void OnRankingPressed() => GetTree().ChangeSceneToFile("res://scenes/ui/ranking_score/ranking_score.tscn");

	private void UnsubscribeFromButtonEvents()
	{
		GetNode<Button>(StartButton).Pressed -= OnStartPressed;
		GetNode<Button>(UpgradesButton).Pressed -= OnUpgradesPressed;
		GetNode<Button>(OptionsButton).Pressed -= OnOptionsPressed;
		GetNode<Button>(QuitButton).Pressed -= OnQuitPressed;
	}
    
	public override void _ExitTree() => UnsubscribeFromButtonEvents();
}
