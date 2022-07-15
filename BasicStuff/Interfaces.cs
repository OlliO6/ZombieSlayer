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