using Godot;
using System;
using OnReadyCs;

public partial class ScoreContainer : HBoxContainer
{
	[Export]
	public int RankingPosition { get; set; }

	[Export]
	public string RankingName { get; set; }

	[Export]
	public int RankingScore { get; set; }

	[OnReady("%Position")]
	public Label LabelPosition { get; set; }

	[OnReady("%Name")]
	public Label LabelName { get; set; }

	[OnReady("%Score")]
	public Label LabelScore { get; set; }

    public override void _Ready() => this.InitializeOnReadyFields();

	public void SetValues()
	{
		LabelPosition.Text = $"{RankingPosition}";
		LabelName.Text = $"{RankingName}";
		LabelScore.Text = $"{RankingScore}";
	}
}
