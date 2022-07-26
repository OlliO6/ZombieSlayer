using Godot;
using Additions;

public class Zombie : KinematicBody2D, IDamageable, IKillable, IHealth
{
    [Export] private PackedScene coinScene;
    [Export] private int coinsAmount = 2;
    [Export] private Vector2 movementSpeedRange;

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

    #region FlashTween Reference

    #region AudioPlayer Reference

    private AudioStreamPlayer storerForAudioPlayer;
    public AudioStreamPlayer AudioPlayer => this.LazyGetNode(ref storerForAudioPlayer, "AudioPlayer");

    #endregion

    private Tween storerForFlashTween;
    public Tween FlashTween => this.LazyGetNode(ref storerForFlashTween, "FlashTween");

    #endregion

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        movementSpeed = Random.FloatRange(movementSpeedRange);

        AnimTree.SetParam("State/current", 1);
        float weight = Mathf.InverseLerp(movementSpeedRange.x, movementSpeedRange.y, movementSpeed);
        float runSpeedScale = Mathf.Lerp(0.6f, 1, weight);

        AnimTree.SetParam("RunSpeed/scale", runSpeedScale);

        FlashTween.Connect("tween_all_completed", AnimTree, "set", new("parameters/RunSpeed/scale", runSpeedScale));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead || FlashTween.IsActive()) return;

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

        FlashTween.InterpolateProperty(Sprite.Material, "shader_param/flashStrenght", 0.9f, 0f, 0.15f, Tween.TransitionType.Cubic, Tween.EaseType.In);
        FlashTween.Start();
        AnimTree.SetParam("RunSpeed/scale", 0);

        AudioPlayer.Play();

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
