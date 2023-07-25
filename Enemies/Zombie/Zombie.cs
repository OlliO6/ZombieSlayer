namespace Enemies;
using Additions;
using Godot;
using static EnemyUtilities;

[Additions.Debugging.DefaultColor("LightGreen")]
public class Zombie : KinematicBody2D, IEnemy, IDamageable, IKillable, IHealth, IStunnable
{
    [Signal] public delegate void Damaged();
    [Signal] public delegate void Died();

    [Export] private int coinsAmount = 2;
    [Export] private Vector2 movementSpeedRange;
    [Export] public float stunTimeAfterBodyDamage = 1;

    public float movementSpeed;
    public int DamageAmount => 1;
    public int ExPoints => 5;

    public int CurrentHealth { get; set; }
    [Export] public int MaxHealth { get; set; }
    public event System.Action<IEnemy> EnemyDied;

    private bool isInvincible;
    float runSpeedScale;

    private AnimationTree storerForAnimTree;
    public AnimationTree AnimTree => this.LazyGetNode(ref storerForAnimTree, "AnimationTree");

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    public bool IsStunned { get; private set; }

    public Timer StunnTimer => GetNode<Timer>("StunnTimer");

    public bool IsDead { get; private set; }

    public override void _Ready()
    {
        BasicSetup(this);
        SetupDespawnOnLevelUpAndNotOnScreen(this, GetNode<VisibilityNotifier2D>("VisibilityDisabler"));
        CurrentHealth = MaxHealth;
        movementSpeed = Random.FloatRange(movementSpeedRange);
        GetNode<DamagingHurtArea>("DamagingHurtArea").AllowDealingDamage = AboutToDealBodyDamage;

        AnimTree.SetParam("State/current", 1);
        float weight = Mathf.InverseLerp(movementSpeedRange.x, movementSpeedRange.y, movementSpeed);
        runSpeedScale = Mathf.Lerp(0.6f, 1, weight);
        StunnTimer.Connect(Constants.timeout, this, nameof(UnStunn));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (IsDead || IsStunned || !Player.Exists ||
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

    private bool AboutToDealBodyDamage(IDamageable to)
    {
        Stunn(stunTimeAfterBodyDamage);
        return true;
    }

    public bool AllowDamageFrom(IDamageDealer from) => !isInvincible;

    public void GetDamage(int amount)
    {
        CurrentHealth -= amount;

        FlashSprite(Sprite);

        EmitSignal(nameof(Damaged));

        if (CurrentHealth <= 0) Die();

        isInvincible = true;
        new TimeAwaiter(this, InvisTime, onCompleted: () => isInvincible = false);
        Stunn(StunTime);
    }

    public void Stunn(float time)
    {
        IsStunned = true;
        StunnTimer.Start(time);
    }

    private void UnStunn()
    {
        IsStunned = false;
        AnimTree.Set("parameters/RunSpeed/scale", runSpeedScale);
    }

    public void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        Debug.Log(this, "Died");

        DeadYSort(this, Sprite, 4);

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
