using Godot;
using System;
using Additions;

public class Zombie : KinematicBody2D, IDamageable, IKillable, IHealth
{
    [Export] private float movementSpeed = 20;
    [Export(PropertyHint.Range, "0,1")] private float movementSpeedRandomness = 0.5f;

    public int DamageAmount => 1;

    public int CurrentHealth { get; set; }
    [Export] public int MaxHealth { get; set; }


    [Signal] public delegate void OnDamaged();
    [Signal] public delegate void OnDied();

    private bool dead;

    #region AnimTree Reference

    private AnimationTree storerForAnimTree;
    public AnimationTree AnimTree => this.LazyGetNode(ref storerForAnimTree, "AnimationTree");

    #endregion

    #region Sprite Reference

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    #endregion

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        movementSpeed = movementSpeed * (1 - (GD.Randf() * movementSpeedRandomness));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead) return;

        Vector2 dirToPlayer = GetDirectionToPlayer();

        Sprite.FlipH = dirToPlayer.x < 0 ? true : false;

        MoveAndSlide(dirToPlayer * movementSpeed);
    }

    private Vector2 GetDirectionToPlayer()
    {
        if (Player.currentPlayer is null)
            return Vector2.Zero;

        return (Player.currentPlayer.GlobalPosition - GlobalPosition).Normalized();
    }

    public void GetDamage(int amount)
    {
        GD.Print($"Zombie got {amount} damage");
        EmitSignal(nameof(OnDamaged));

        CurrentHealth -= amount;

        AnimTree.Set("parameters/Damage/active", true);

        if (CurrentHealth <= 0) Die();
    }
    public void Die()
    {
        EmitSignal(nameof(OnDied));
        dead = true;

        AnimTree.Set("parameters/State/current", 2);
    }
}
