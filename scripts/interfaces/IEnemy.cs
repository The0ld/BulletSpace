using Godot;

public interface IEnemy : ICharacter
{
    public int ScoreGain { get; }
    public DropComponent DropComponentProp { get; set; }
}