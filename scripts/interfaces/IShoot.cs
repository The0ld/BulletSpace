public interface IShoot
{
    float Speed { get; set; }
    bool IsFriend { get; set; }
    bool Impacted { get; set; }
    
    void MoveProjectile(float delta);
    void OnImpact();
    void OnScreenExited();
}