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

public interface ISelectable
{
    public bool IsSelected { get; set; }
}