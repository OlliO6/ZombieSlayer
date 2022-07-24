using Godot;

public interface IKillable
{
    public void Die();
}
public interface IDamageable
{
    public void GetDamage(int amount);
}
public interface IDamageDealer
{
    public int DamageAmount { get; }

    public void DamageReceived(IDamageable to);
}
public interface IHealth
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}

public interface ISelectable
{
    public bool Selected { get; set; }
}