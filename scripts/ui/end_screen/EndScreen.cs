using Godot;
using System;
using OnReadyCs;

public partial class EndScreen : Control
{
	[OnReady("%InputName")]
	public LineEdit InputName { get; private set; }

	[OnReady("/root/MetaProgression")]
	public MetaProgression MetaProgressionProp { get; private set; }

	public ScoreManager ScoreManagerProp { get; private set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		GetNode<Button>("%Continue").Pressed += OnContinuePressed;

		ScoreManagerProp = GetTree().GetFirstNodeInGroup("score") as ScoreManager;

		GetTree().Paused = true;
	}

	public void SetFinalScore()
	{
		var score = ScoreManagerProp.CurrentScore;
		GetNode<Label>("%FinalScore").Text = $"SCORE: {score}";
	}

	public void OnContinuePressed()
	{
		if (InputName.Text != string.Empty)
		{
			MetaProgressionProp.SaveScore(InputName.Text, ScoreManagerProp.CurrentScore);
			MetaProgressionProp.Save();
		}

		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/ui/ranking_score/ranking_score.tscn");
	}

	public override void _ExitTree() => GetNode<Button>("%Continue").Pressed -= OnContinuePressed;
}
