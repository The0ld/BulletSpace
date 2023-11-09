using Godot;
using System;
using OnReadyCs;

public partial class PauseMenu : CanvasLayer
{
	[OnReady("%ResumeButton")]
	public Button ResumeButton { get; set; }

	[OnReady("%OptionsButton")]
	public Button OptionsButton { get; set; }

	[OnReady("%QuitButton")]
	public Button QuitButton { get; set; }

	public PackedScene OptionScene { get; private set; } = GD.Load<PackedScene>("res://scenes/ui/options_menu/options_menu.tscn");

	public bool IsClosing { get; private set; } = false;

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		GetTree().Paused = true;

		ResumeButton.Pressed += OnResumeButtonPressed;
		OptionsButton.Pressed += OnOptionsButtonPressed;
		QuitButton.Pressed += OnQuitButtonPressed;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause") && !IsClosing)
		{
			Close();
			GetTree().Root.SetInputAsHandled();
		}
    }

	private void Close()
	{
		if (IsClosing) return;
		
		IsClosing = true;

		GetTree().Paused = false;
		QueueFree();
	}

	private void OnOptionsButtonPressed()
	{
		Node rootNode = GetTree().Root;
		
		var optionsMenuInstance = OptionScene.Instantiate();
		rootNode.AddChild(optionsMenuInstance);
		
		Visible = false;
		
		(optionsMenuInstance as OptionsMenu).BackPressed += () => OnOptionBackPressed(optionsMenuInstance);
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/ui/main_menu/main_menu.tscn");
	}

	private void OnResumeButtonPressed() => Close();

	public void OnOptionBackPressed(Node optionsMenu)
	{
		optionsMenu.QueueFree();

		Visible = true;
	}

    public override void _ExitTree()
    {
        ResumeButton.Pressed -= OnResumeButtonPressed;
		OptionsButton.Pressed -= OnOptionsButtonPressed;
		QuitButton.Pressed -= OnQuitButtonPressed;
    }
}
