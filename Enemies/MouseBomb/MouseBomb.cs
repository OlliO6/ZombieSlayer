namespace Enemies;
using System;
using System.Collections.Generic;
using Additions;
using Godot;
using static EnemyUtilities;

public class MouseBomb : KinematicBody2D, IEnemy, IDamageable, IKillable, IHealth
{
    [Signal] public delegate void Damaged();
    [Signal] public delegate void Died();
    [Signal] public delegate void Detonated();
    public event Action<IEnemy> EnemyDied;

    [TroughtEditor, Export] public float detonationSpeedFactor;

    [Export] private int coinsAmount;
    [Export] private float movementSpeed;

    private bool isInvincible, dead, isStunned, isDetonating;
    private AnimationTree storerForAnimTree;
    private Sprite storerForSprite;


    public AnimationTree AnimTree => this.LazyGetNode(ref storerForAnimTree, "AnimationTree");
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");
    public int ExPoints => 3;
    public int CurrentHealth { get; set; }
    [Export] public int MaxHealth { get; set; }

    public override void _Ready()
    {
        BasicSetup(this);
        CurrentHealth = MaxHealth;

        AnimTree.SetParam("RunSpeed/scale", 1f);

        GetNode("DetonationArea").Connect("body_entered", this, nameof(DetonationZoneEntered));
    }

    private void DetonationZoneEntered(Node node)
    {
        if (dead || node is not Player) return;

        AnimTree.SetParam("State/current", 1);
        isDetonating = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead || isStunned || !Player.Exists ||
                Position.DistanceTo(Player.currentPlayer.Position) < NoMoveDist)
        {
            AnimTree.SetParam("RunSpeed/scale", 0);
            return;
        }

        AnimTree.SetParam("RunSpeed/scale", 1);
        Vector2 dirToPlayer = GetDirectionToPlayer(this);
        Sprite.FlipH = dirToPlayer.x > 0; // Greater because sprite is flipped
        MoveAndSlide(dirToPlayer * movementSpeed * (isDetonating ? detonationSpeedFactor : 1));
    }

    public bool AllowDamageFrom(IDamageDealer from) => !isInvincible;

    public void GetDamage(int amount)
    {
        CurrentHealth -= amount;

        FlashSprite(Sprite);

        EmitSignal(nameof(Damaged));

        if (CurrentHealth <= 0)
            Die();

        isStunned = true;
        isInvincible = true;
        new TimeAwaiter(this, InvisTime, onCompleted: () => isInvincible = false);
        new TimeAwaiter(this, StunTime, onCompleted: () =>
                {
                    isStunned = false;
                    AnimTree.Set("parameters/RunSpeed/scale", 1f);
                });
    }

    public void Die()
    {
        dead = true;
        AnimTree.SetParam("State/current", 2);
        AnimTree.SetParam("DiyingType/current", 0);

        DeadYSort(this, Sprite, 10);

        SpawnCoins(this, coinsAmount);
        EmitSignal(nameof(Died));
        EnemyDied?.Invoke(this);

        Debug.Log(this, "Died");
    }

    public void Explode()
    {
        dead = true;
        AnimTree.SetParam("State/current", 2);
        AnimTree.SetParam("DiyingType/current", 1);

        DeadYSort(this, Sprite, 10);

        EmitSignal(nameof(Died));
        EnemyDied?.Invoke(this);

        Debug.Log(this, "Exploded");
    }

    [TroughtEditor]
    public void DetonationFinished()
    {
        EmitSignal(nameof(Detonated));
        Explode();
    }

    [TroughtEditor]
    public void DeleteEverythingButSprite()
    {
        Vector2 spritePos = Sprite.GlobalPosition;
        RemoveChild(Sprite);
        GetParent().AddChild(Sprite);
        Sprite.GlobalPosition = spritePos;

        VisibilityDisabler visibilityDisabler = GetNode<VisibilityDisabler>("VisibilityDisabler");
        RemoveChild(visibilityDisabler);
        Sprite.AddChild(visibilityDisabler);
        visibilityDisabler.Target = Sprite;

        QueueFree();
    }
}
