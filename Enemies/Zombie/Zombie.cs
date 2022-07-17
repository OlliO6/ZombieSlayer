using Godot;
using System;
using Additions;

public class Zombie : KinematicBody2D, IDamageable, IKillable, IHealth
{
    public int DamageAmount => 1;

    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    #region AnimTree Reference

    private AnimationTree storerForAnimTree;
    public AnimationTree AnimTree => this.LazyGetNode(ref storerForAnimTree, "AnimationTree");

    #endregion

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }


    public void GetDamage(int amount)
    {
        GD.Print($"Zombie got {amount} damage");
        CurrentHealth -= amount;

        if (CurrentHealth <= 0) Die();
    }
    public void Die()
    {
        throw new NotImplementedException();
    }
}
