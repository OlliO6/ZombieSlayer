using Additions;
using Godot;
using System.Collections.Generic;
using System.Linq;

public class Player : KinematicBody2D, IDamageable, IKillable, IHealth
{
    public static Player currentPlayer;
    [Export] public float movementSpeed, invincibilityTime;
    [Export] public float damageMultiplier = 1;
    [Export] public int startCoins = 0;
    [Export] private float startMagnetSize = 4;

    [Export] public int MaxHealth { get; set; }

    public int CurrentHealth
    {
        get => storerForCurrentHealth;
        set
        {
            storerForCurrentHealth = value;
            EmitSignal(nameof(HealthChanged));
            if (value <= 0) Die();
        }
    }
    private int storerForCurrentHealth;

    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            EmitSignal(nameof(CoinsAmountChanged), value);
        }
    }
    private int coins;

    public float MagnetAreaSize { get => MagnetArea.Size; set => MagnetArea.Size = value; }

    [Signal] public delegate void CoinsAmountChanged(int amount);
    [Signal] public delegate void InvincibilityStarted();
    [Signal] public delegate void InvincibilityEnded();
    [Signal] public delegate void HealthChanged();


    #region AnimationTree Reference

    private AnimationTree storerForAnimationTree;
    public AnimationTree AnimationTree => this.LazyGetNode(ref storerForAnimationTree, "AnimationTree");

    #endregion

    #region Sprite Reference

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    #endregion

    #region MagnetArea Reference

    private MagnetArea storerForMagnetArea;
    public MagnetArea MagnetArea => this.LazyGetNode(ref storerForMagnetArea, "MagnetArea");

    #endregion

    #region Weapons Reference

    private WeaponSwitcher storerForWeapons;
    public WeaponSwitcher Weapons => this.LazyGetNode(ref storerForWeapons, "WeaponPivot/WeaponSwitcher");

    #endregion

    #region Upgrades Reference

    private Node storerForUpgrades;
    public Node Upgrades => this.LazyGetNode(ref storerForUpgrades, "Upgrades");

    #endregion

    #region DiceInventory Reference

    private DiceInventory storerForDiceInventory;
    public DiceInventory DiceInventory => this.LazyGetNode(ref storerForDiceInventory, "DiceInventory");

    #endregion

    public bool isInvincible;

    public override void _EnterTree()
    {
        currentPlayer = this;
    }

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        Coins = startCoins;
        MagnetAreaSize = startMagnetSize;

        DebugOverlay.AddWatcher(this, nameof(MagnetAreaSize), showTargetName: false);
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 inputVector = InputManager.GetMovementInput();

        Move(inputVector, delta);
        Animate(inputVector);
    }

    private void Move(Vector2 inputVector, float delta)
    {
        MoveAndSlide(inputVector * movementSpeed);
    }

    private void Animate(Vector2 inputVector)
    {
        float lenght = inputVector.Length();

        if (lenght > 0.2f)
        {
            AnimationTree.Set("parameters/MovementState/current", 1);
            AnimationTree.Set("parameters/RunSpeed/scale", lenght.Clamp01());
        }
        else
        {
            AnimationTree.Set("parameters/MovementState/current", 0);
        }
    }

    public void GetDamage(int amount)
    {
        if (isInvincible)
            return;

        GD.Print($"Player got {amount} damage");

        isInvincible = true;
        EmitSignal(nameof(InvincibilityStarted));
        Sprite.SetShaderParam("blinking", true);

        CurrentHealth -= amount;

        GetNode<Timer>("InvincibilityTimer").Start(invincibilityTime);
    }
    [TroughtSignal]
    private void OnInvincibilityTimeEnded()
    {
        isInvincible = false;
        EmitSignal(nameof(InvincibilityEnded));
        Sprite.SetShaderParam("blinking", false);
    }

    public void Die()
    {
        SceneManager.LoadMenu();
    }


    public void AddDice(Dice dice) => DiceInventory.AddDice(dice);
    public IEnumerable<Dice> GetWorkingDices() => DiceInventory.GetWorkingDices();
    public IEnumerable<Dice> GetBrokenDices() => DiceInventory.GetBrokenDices();

    public void AddUpgrade(Upgrade upgrade) => Upgrades.AddChild(upgrade);
    public IEnumerable<Upgrade> GetUpgrades() => Upgrades.GetChildren<Upgrade>();

    public void AddWeapon(WeaponBase weapon) => Weapons.AddWeapon(weapon);
    public IEnumerable<WeaponBase> GetWeapons() => Weapons.GetChildren<WeaponBase>();
}
