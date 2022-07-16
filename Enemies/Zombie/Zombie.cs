using Godot;
using System;

public class Zombie : KinematicBody2D, IDamageable, IKillable, IDamageDealer
{
    public int DamageAmount => 1;

    public void GetDamage(int amount)
    {
        GD.Print($"Zombie got {amount} damage");
    }
    public void Die()
    {
        throw new NotImplementedException();
    }

    public void DamageReceived(IDamageable to)
    {
        GD.Print($"Zombie dealed {DamageAmount} damage to {(to as Node).Name}");
    }
}
