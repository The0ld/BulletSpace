using Godot;
using OnReadyCs;
using System;

public partial class MusicBG : AudioStreamPlayer
{
	[OnReady("WaitRepeat")]
	public Timer WaitRepeat { get; set; }

	public override void _Ready()
	{
		this.InitializeOnReadyFields();

		Finished += OnFinished;
		WaitRepeat.Timeout += OnTimeout;
	}

	private void OnFinished() => WaitRepeat.Start();
	
	private void OnTimeout() => Play();

    public override void _ExitTree()
    {
        Finished -= OnFinished;
		WaitRepeat.Timeout -= OnTimeout;
    }
}
