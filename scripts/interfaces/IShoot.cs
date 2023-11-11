using Godot;
using System;

public interface IShoot
{
    float Speed { get; set; }
    bool IsFriend { get; set; }
    bool Impacted { get; set; }
    public Area2D HitBoxComponent { get; }
    
    void MoveProjectile(float delta);
    void OnImpact();
    void OnScreenExited();
}