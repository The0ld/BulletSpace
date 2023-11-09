using Godot;
using OnReadyCs;
using System;

public partial class SoundButtonComponent : Button
{
	[OnReady("%AudioStreamButton")]
	public AudioStreamPlayer AudioStreamButton { get; private set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();
		Pressed += OnPressed;
	}

	private void Play() => AudioStreamButton.Play();

	public void OnPressed() => Play();

    public override void _ExitTree() => Pressed -= OnPressed;
}
