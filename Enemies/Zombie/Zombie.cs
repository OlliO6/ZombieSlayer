using Additions;
using Godot;

[Additions.Debugging.DefaultColor("LightGreen")]
public class Zombie : KinematicBody2D, IEnemy, IDamageable, IKillable, IHealth
{
    public const float InvisTime = 0.075f, StunTime = 0.15f;

    [Export] private PackedScene coinScene;
    [Export] private int coinsAmount = 2;
    [Export] private Vector2 movementSpeedRange;

    public float movementSpeed;
    public int DamageAmount => 1;
    public int ExPoints => 5;

    public int CurrentHealth { get; set; }
    [Export] public int MaxHealth { get; set; }

    [Signal] public delegate void Damaged();
    [Signal] public delegate void Died();
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

    #region AudioPlayer Reference

    private AudioStreamPlayer storerForAudioPlayer;
    public AudioStreamPlayer AudioPlayer => this.LazyGetNode(ref storerForAudioPlayer, "HurtAudioPlayer");

    #endregion

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        movementSpeed = Random.FloatRange(movementSpeedRange);

        AnimTree.SetParam("State/current", 1);
        float weight = Mathf.InverseLerp(movementSpeedRange.x, movementSpeedRange.y, movementSpeed);
        runSpeedScale = Mathf.Lerp(0.6f, 1, weight);

        AnimTree.SetParam("RunSpeed/scale", runSpeedScale);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead || isStunned) return;

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

    public bool AllowDamageFrom(IDamageDealer from) => !isInvincible;

    public void GetDamage(int amount)
    {
        CurrentHealth -= amount;
        AnimateDamage();

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

        void AnimateDamage()
        {
            var tween = CreateTween();
            tween.Chain()
                    .TweenProperty(Sprite, "material:shader_param/flashStrenght", 0.9f, 0.05f)
                    .From(0f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.In);

            tween.TweenProperty(Sprite, "material:shader_param/flashStrenght", 0f, 0.15f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.In);

            AnimTree.SetParam("RunSpeed/scale", 0);
            AudioPlayer.Play();
        }
    }

    public void Die()
    {
        dead = true;
        Debug.Log(this, "Died");

        // better y sort
        Sprite.Offset += Vector2.Down * 5;
        Position += Vector2.Up * 5;

        AnimTree.SetParam("State/current", 2);

        if (Player.currentPlayer is not null) Player.currentPlayer.Leveling.CurrentXp += ExPoints;

        SpawnCoins();
        EmitSignal(nameof(Died));
        EnemyDied?.Invoke(this);
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
