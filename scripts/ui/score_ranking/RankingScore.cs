using Godot;
using Godot.Collections;
using System;
using OnReadyCs;

public partial class RankingScore : CanvasLayer
{
	[OnReady("%ScoreList")]
	public VBoxContainer ScoreList { get; private set; }

	[OnReady("%Back")]
	public Button Back { get; private set; }

	public PackedScene ScoreContainer { get; private set; } = GD.Load<PackedScene>("res://scenes/ui/score_container/score_container.tscn");

	public Dictionary[] Scores { get; private set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		Back.Pressed += OnBackPressed;
		
		Scores = GetNode<MetaProgression>("/root/MetaProgression").GetScoreRanking();

		SetScores();
	}

	private void SetScores()
	{
		int rankingPosition = 0;

		foreach (Dictionary score in Scores)
		{
			rankingPosition++;

			var ScoreContainerInstance = ScoreContainer.Instantiate();
			ScoreList.AddChild(ScoreContainerInstance);

			(ScoreContainerInstance as ScoreContainer).RankingPosition = rankingPosition;
			(ScoreContainerInstance as ScoreContainer).RankingName = (string)score["name"];
			(ScoreContainerInstance as ScoreContainer).RankingScore = (int)score["score"];

			(ScoreContainerInstance as ScoreContainer).SetValues();

			if (rankingPosition >= 10)
				break;
		}
	}

	private void OnBackPressed() => GetTree().ChangeSceneToFile("res://scenes/ui/main_menu/main_menu.tscn");

    public override void _ExitTree() => Back.Pressed -= OnBackPressed;
}
