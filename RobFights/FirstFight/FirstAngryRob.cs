namespace Enemies;
using Godot;
using System;

public class FirstAngryRob : Node2D, IDamageable, IHealth, IKillable, IEnemy
{
    [Export] public int MaxHealth { get; set; } = 100;

    [Export] public int ExPoints { get; set; } = 20;
    [Export] public int Coins { get; set; } = 30;

    public bool Enabled { get; set; }

    private int _currentHealth;

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            if (CurrentHealth <= 0)
                Die();
        }
    }

    public bool IsDead { get; private set; }

    public event Action<IEnemy> EnemyDied;

    public bool AllowDamageFrom(IDamageDealer from) => Enabled;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        EnemyUtilities.BasicSetup(this);
    }

    public void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        EnemyDied?.Invoke(this);
        EnemyUtilities.SpawnCoins(this, Coins);
    }

    public void GetDamage(int amount)
    {
        CurrentHealth -= amount;
        EnemyUtilities.FlashSprite(GetNode<Sprite>("Sprite"));
    }
}
