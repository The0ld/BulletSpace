using Godot;
using System;

public interface ICollectible
{
    Resource CollectibleResource { get; set; }
    void TweenCollectWithStartPosition(float percent);
    void DisableCollision();
    void Collect();
    void TweenCollect(float percent, Vector2 startPosition);
}