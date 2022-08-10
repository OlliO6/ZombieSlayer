using Godot;

public interface IEnemy : IKillable
{
    public int ExPoints { get; }
    public event System.Action OnDied;
}