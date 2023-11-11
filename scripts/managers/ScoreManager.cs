using Godot;
using System;

public partial class ScoreManager : Node
{
    [Signal]
    public delegate void ScoreUpdatedEventHandler(int score);

    public int CurrentScore { get; private set; }

    public override void _Ready() =>  GetNode<GameEvents>("/root/GameEvents").ScoreIncreased += OnScoreIncreased;

    private void IncreaseScore(int score)
    {
        CurrentScore += score;
        EmitSignal(nameof(ScoreUpdated), CurrentScore);
    }

    private void OnScoreIncreased(int score) => IncreaseScore(score);

    public override void _ExitTree() => GetNode<GameEvents>("/root/GameEvents").ScoreIncreased -= OnScoreIncreased;
}
