namespace Enemies;
using Additions;
using Godot;
using static EnemyUtilities;

[Additions.Debugging.DefaultColor("LightGreen")]
public class Zombie : KinematicBody2D, IEnemy, IDamageable, IKillable, IHealth
{
    [Signal] public delegate void Damaged();
    [Signal] public delegate void Died();

    [Export] private int coinsAmount = 2;
    [Export] private Vector2 movementSpeedRange;

    public float movementSpeed;
    public int DamageAmount => 1;
    public int ExPoints => 5;

    public int CurrentHealth { get; set; }
    [Export] public int MaxHealth { get; set; }
    public event System.Action<IEnemy> EnemyDied;

    private bool dead, isInvincible;
    private bool isStunned;
    float runSpeedScale;


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
        BasicSetup(this);
        CurrentHealth = MaxHealth;
        movementSpeed = Random.FloatRange(movementSpeedRange);

        AnimTree.SetParam("State/current", 1);
        float weight = Mathf.InverseLerp(movementSpeedRange.x, movementSpeedRange.y, movementSpeed);
        runSpeedScale = Mathf.Lerp(0.6f, 1, weight);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead || isStunned || !Player.Exists ||
                Position.DistanceTo(Player.currentPlayer.Position) < NoMoveDist)
        {
            AnimTree.SetParam("RunSpeed/scale", 0);
            return;
        }
        AnimTree.SetParam("RunSpeed/scale", runSpeedScale);

        Vector2 dirToPlayer = GetDirectionToPlayer(this);
        Sprite.FlipH = dirToPlayer.x < 0;
        MoveAndSlide(dirToPlayer * movementSpeed);
    }

    public bool AllowDamageFrom(IDamageDealer from) => !isInvincible;

    public void GetDamage(int amount)
    {
        CurrentHealth -= amount;

        FlashSprite(Sprite);

        EmitSignal(nameof(Damaged));

        if (CurrentHealth <= 0) Die();

        isStunned = true;
        isInvincible = true;
        new TimeAwaiter(this, InvisTime, onCompleted: () => isInvincible = false);
        new TimeAwaiter(this, StunTime, onCompleted: () =>
                {
                    isStunned = false;
                    AnimTree.Set("parameters/RunSpeed/scale", runSpeedScale);
                });
    }

    public void Die()
    {
        dead = true;
        Debug.Log(this, "Died");

        DeadYSort(this, Sprite, 5);

        AnimTree.SetParam("State/current", 2);

        SpawnCoins(this, coinsAmount);
        EmitSignal(nameof(Died));
        EnemyDied?.Invoke(this);
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
