using Godot;

public interface IKillable
{
    public void Die();
}
public interface IDamageable
{
    public void GetDamage(int amount);
    public bool AllowDamageFrom(IDamageDealer from);
}
public interface IStunnable
{
    bool IsStunned { get; }
    Timer StunnTimer { get; }
    void Stunn(float time);
}
public interface IDamageDealer
{
    public int DamageAmount { get; }
    public bool AllowDamageTo(IDamageable to);
}
public interface IHealth
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}

public interface IInteractable
{
    public Vector2 Position { get; }
    public void Interact();
    public void Select();
    public void Deselect();
}


public interface ISelectable
{
    public bool IsSelected { get; set; }
}