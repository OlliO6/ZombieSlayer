using Godot;
using System;
using Additions;

public class Zombie : KinematicBody2D, IDamageable, IKillable, IHealth
{
    [Export] private PackedScene coinScene;
    [Export] private int coinsAmount = 2;
    [Export] private float maxMovementSpeed = 20;
    [Export(PropertyHint.Range, "0,1")] private float movementSpeedRandomness = 0.5f;

    public int DamageAmount => 1;

    public float movementSpeed;

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
        movementSpeed = maxMovementSpeed * (1 - (GD.Randf() * movementSpeedRandomness));

        AnimTree.SetParam("State/current", 1);
        float weight = Mathf.InverseLerp(maxMovementSpeed * (1 - movementSpeedRandomness), maxMovementSpeed, movementSpeed);
        AnimTree.SetParam("RunSpeed/scale", Mathf.Lerp(0.6f, 1, weight));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead || (bool)AnimTree.GetParam("Damage/active")) return;

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

        AnimTree.SetParam("Damage/active", true);

        if (CurrentHealth <= 0) Die();
    }
    public void Die()
    {
        EmitSignal(nameof(OnDied));
        dead = true;

        AnimTree.SetParam("State/current", 2);

        SpawnCoins();
    }

    private void SpawnCoins()
    {
        for (int i = 0; i < coinsAmount; i++)
        {
            Coin coin = coinScene.Instance<Coin>();

            GetTree().CurrentScene.CallDeferred("add_child", coin);

            coin.GlobalPosition = GlobalPosition;
            coin.Launch();
        }
    }
}
