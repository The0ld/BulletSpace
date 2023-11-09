using Godot;
using System;
using OnReadyCs;

public partial class OptionsMenu : CanvasLayer
{
	[Signal]
	public delegate void BackPressedEventHandler();

	[OnReady("%WindowButton")]
	public Button WindowButton { get; set; }

	[OnReady("%Back")]
	public Button BackButton { get; set; }

	[OnReady("%SfxSlider")]
	public HSlider SfxSlider { get; set; }

	[OnReady("%MusicSlider")]
	public HSlider MusicSlider { get; set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		BackButton.Pressed += OnBackButtonPressed;
		WindowButton.Pressed += OnWindowButtonPressed;
		SfxSlider.ValueChanged += (double value) => OnAudioSliderChanged(value, "SFX");
		MusicSlider.ValueChanged += (double value) => OnAudioSliderChanged(value, "Music");

		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		WindowButton.Text = "WINDOWED";
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
			WindowButton.Text = "FULLSCREEN";

		SfxSlider.Value = GetBusVolumePercent("SFX");
		MusicSlider.Value = GetBusVolumePercent("Music");
	}

	public float GetBusVolumePercent(string busName)
	{
		int busIndex = AudioServer.GetBusIndex(busName);
		float volumeDb = AudioServer.GetBusVolumeDb(busIndex);

		return Mathf.DbToLinear(volumeDb);
	}

	public void SetBusVolumePercent(string busName, float percent)
	{
		int busIndex = AudioServer.GetBusIndex(busName);
		float volumeDb = Mathf.LinearToDb(percent);
		AudioServer.SetBusVolumeDb(busIndex, volumeDb);
	}

	public void OnWindowButtonPressed()
	{
		var mode = DisplayServer.WindowGetMode();

		if (mode != DisplayServer.WindowMode.Fullscreen)
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		else
		{
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}

		UpdateDisplay();
	}

	public void OnAudioSliderChanged(double sliderVolume, string busName) => SetBusVolumePercent(busName, (float)sliderVolume);

	public void OnBackButtonPressed() => EmitSignal(nameof(BackPressed));

    public override void _ExitTree()
    {
        BackButton.Pressed -= OnBackButtonPressed;
		WindowButton.Pressed -= OnWindowButtonPressed;
    }
}
