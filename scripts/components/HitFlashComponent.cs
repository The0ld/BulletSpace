using Godot;
using System;

public partial class HitFlashComponent : Node
{
    [Export]
    public Node HealthComponentProp { get; set; }

    [Export]
    public Sprite2D Sprite { get; set; }

    [Export]
    public ShaderMaterial HitFlashMaterial { get; set; }

    public Tween HitFlashTween { get; set; }

    public override void _Ready()
    {
        (HealthComponentProp as HealthComponent).HealthDecreased += OnHealthDecreased;

        Sprite.Material = HitFlashMaterial;
    }

    public void OnHealthDecreased()
    {
        if (HitFlashTween != null && HitFlashTween.IsValid()) HitFlashTween.Kill();

        (Sprite.Material as ShaderMaterial).SetShaderParameter("lerp_percent", 1.0);
        HitFlashTween = CreateTween();
        HitFlashTween.TweenProperty(Sprite.Material, "shader_parameter/lerp_percent", 0.0f, 0.25f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);
    }

    public override void _ExitTree() => (HealthComponentProp as HealthComponent).HealthDecreased -= OnHealthDecreased;
}
