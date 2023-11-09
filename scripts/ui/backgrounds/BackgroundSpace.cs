using Godot;
using OnReadyCs;
using System;

public partial class BackgroundSpace : Node
{
    [Export]
    public float Speed { get; set; } = 3;

    [OnReady("%Back")]
    public ParallaxBackground Back { get; set; }

    [OnReady("%Stars")]
    public ParallaxBackground Stars { get; set; }

    [OnReady("%Planet1")]
    public ParallaxBackground Planet1 { get; set; }

    [OnReady("%Planet2")]
    public ParallaxBackground Planet2 { get; set; }

    public override void _Ready() => this.InitializeOnReadyFields();

    public override void _PhysicsProcess(double delta) => ParallaxBg(delta);

    public void ParallaxBg(double delta)
    {
        Back.ScrollBaseOffset = new Vector2(Mathf.Lerp(Back.ScrollBaseOffset.X, Back.ScrollBaseOffset.X - 16, Speed * (float)delta), 0);
        Stars.ScrollBaseOffset = new Vector2(Mathf.Lerp(Stars.ScrollBaseOffset.X, Stars.ScrollBaseOffset.X - 24, Speed * (float)delta), 0);
        Planet1.ScrollBaseOffset = new Vector2(Mathf.Lerp(Planet1.ScrollBaseOffset.X, Planet1.ScrollBaseOffset.X - 32, Speed * (float)delta), 0);
        Planet2.ScrollBaseOffset = new Vector2(Mathf.Lerp(Planet2.ScrollBaseOffset.X, Planet2.ScrollBaseOffset.X - 32, Speed * (float)delta), 0);
    }
}
