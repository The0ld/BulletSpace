using Godot;
using System;

public partial class LevelOne : Node
{
    public PackedScene EndScreen { get; private set; } = GD.Load<PackedScene>("res://scenes/ui/end_screen/end_screen.tscn");

    public PackedScene PauseMenuScene { get; private set; } = GD.Load<PackedScene>("res://scenes/ui/pause_menu/pause_menu.tscn");

    public override void _Ready() => GetNode<GameEvents>("/root/GameEvents").GameOver += OnGameOver;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
		{
			AddChild(PauseMenuScene.Instantiate());
			GetTree().Root.SetInputAsHandled();
		}
    }

    private void GameOver()
    {
        var endScreenInstance = EndScreen.Instantiate();
		AddChild(endScreenInstance);

        (endScreenInstance as EndScreen).SetFinalScore();
    }

    private void OnGameOver() => GameOver();

    public override void _ExitTree() => GetNode<GameEvents>("/root/GameEvents").GameOver -= OnGameOver;
}
