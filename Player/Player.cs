using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;
using Leveling;
using Shaking;

[Additions.Debugging.DefaultColor(nameof(Colors.LightBlue), nameof(Colors.DeepSkyBlue))]
public class Player : KinematicBody2D, IDamageable, IKillable, IHealth
{
    public static Player currentPlayer;
    public static bool Exists => IsInstanceValid(currentPlayer);

    [Signal] public delegate void CoinsAmountChanged(int amount);
    [Signal] public delegate void LevelChanged(int to);
    [Signal] public delegate void DeathStarted();
    [Signal] public delegate void DeathEnded();
    [Signal] public delegate void Damaged();
    [Signal] public delegate void InvincibilityStarted();
    [Signal] public delegate void InvincibilityEnded();
    [Signal] public delegate void HealthChanged();

    [Export] public float movementSpeed, invincibilityTime;
    [Export] public float damageMultiplier = 1;
    [Export] public int startCoins = 0;
    [Export] private float startMagnetSize = 4;
    [Export] private ShakeProfile damageShake, dieShake;

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
    #region WeaponInv Reference

    private WeaponSwitcher storerForWeapons;
    public WeaponSwitcher WeaponInv => this.LazyGetNode(ref storerForWeapons, "WeaponSwitcher");

    #endregion
    #region UpgradeHolder Reference

    private Node storerForUpgrades;
    public Node UpgradeHolder => this.LazyGetNode(ref storerForUpgrades, "Upgrades");

    #endregion
    #region DiceInventory Reference

    private DiceInventory storerForDiceInventory;
    public DiceInventory DiceInventory => this.LazyGetNode(ref storerForDiceInventory, "DiceInventory");

    #endregion
    #region Leveling Reference

    private LevelingSystem storerForLeveling;
    public LevelingSystem Leveling => this.LazyGetNode(ref storerForLeveling, "Leveling");

    #endregion
    #region CamShaker Reference

    private CamShaker storerForCamShaker;
    public CamShaker CamShaker => this.LazyGetNode(ref storerForCamShaker, "CamShaker");

    #endregion

    public bool isDead;
    public bool isInvincible;
    public List<IInteractable> interactablesInRange = new(1);
    public IInteractable currentInteractable;

    private int storerForCurrentHealth;
    private int coins;

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
    public float MagnetAreaSize
    {
        get => MagnetArea.Size;
        set => MagnetArea.Size = value;
    }
    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            EmitSignal(nameof(CoinsAmountChanged), value);
        }
    }

    public override void _EnterTree()
    {
        currentPlayer = this;

        InputManager.InteractPressed += OnInteractPressed;
    }

    public override void _ExitTree()
    {
        if (currentPlayer == this) currentPlayer = null;

        InputManager.InteractPressed -= OnInteractPressed;
    }

    public override void _Ready()
    {
        Coins = startCoins;
        MagnetAreaSize = startMagnetSize;
        Heal();

        ToSignal(this, nameof(DeathEnded)).OnCompleted(SceneManager.LoadMenu);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (isDead) return;

        InputManager.GetMovementInput(out Vector2 inputVector, out float lenght);

        Move(inputVector, delta);
        Animate(inputVector, lenght);
        UpdateInteractablesInRange();
    }

    private void OnInteractPressed() => currentInteractable?.Interact();

    private void UpdateInteractablesInRange()
    {
        if (interactablesInRange.Count is 0)
        {
            currentInteractable?.Deselect();
            currentInteractable = null;
            return;
        }

        IInteractable nextSelect = interactablesInRange
                .OrderBy((IInteractable interactable) => Position.DistanceSquaredTo(interactable.Position))
                .First();

        if (currentInteractable == nextSelect) return;

        currentInteractable?.Deselect();
        nextSelect.Select();
        currentInteractable = nextSelect;
    }

    public void AddInteractable(IInteractable interactable)
    {
        if (interactablesInRange.Contains(interactable)) return;
        interactablesInRange.Add(interactable);
    }
    public void RemoveInteractable(IInteractable interactable)
    {
        if (!interactablesInRange.Contains(interactable)) return;
        interactablesInRange.Remove(interactable);
        if (currentInteractable == interactable) currentInteractable?.Deselect();
    }

    private void Move(Vector2 inputVector, float delta)
    {
        MoveAndSlide(inputVector * movementSpeed);
    }

    private void Animate(Vector2 inputVector, float lenght)
    {
        if (lenght > 0.2f)
        {
            AnimationTree.Set("parameters/State/current", 1);
            AnimationTree.Set("parameters/RunSpeed/scale", Mathf.Lerp(0.3f, 1, lenght));
        }
        else
        {
            AnimationTree.Set("parameters/State/current", 0);
        }
    }

    public void ShakeCam(ShakeProfile profile) => CamShaker.Shake(profile);
    public void ShakeCam(ShakeProfile profile, float ampAndTimeFactor) => CamShaker.Shake(profile, ampAndTimeFactor);
    public void ShakeCam(ShakeProfile profile, float ampFactor, float timeFactor) => CamShaker.Shake(profile, ampFactor, timeFactor);

    public bool AllowDamageFrom(IDamageDealer from) => true;

    public void GetDamage(int amount)
    {
        if (isInvincible || isDead)
            return;

        if (GameStats.Current.healthUnlocked) CurrentHealth -= amount;

        ShakeCam(damageShake);

        AnimationTree.Set("parameters/Damage/active", true);
        EmitSignal(nameof(Damaged));
        Debug.LogU(this, $"Got {amount} damage");

        if (isDead) return;

        isInvincible = true;
        EmitSignal(nameof(InvincibilityStarted));
        Sprite.SetShaderParam("blinking", true);

        new TimeAwaiter(this, invincibilityTime,
                onCompleted: () =>
                {
                    isInvincible = false;
                    EmitSignal(nameof(InvincibilityEnded));
                    Sprite.SetShaderParam("blinking", false);
                });
    }

    public void Die()
    {
        isDead = true;

        ShakeCam(dieShake);

        PauseMode = PauseModeEnum.Process;
        GetTree().Paused = true;

        Debug.LogU(this, $"Is dying");
        AnimationTree.Set("parameters/State/current", 2);
    }

    public void EndDeath()
    {
        if (isDead) EmitSignal(nameof(DeathEnded));
    }

    [TroughtEditor]
    private void OnLevelChanged()
    {
        EmitSignal(nameof(LevelChanged), Leveling.CurrentLevelIndex);
    }

    public void Heal() => CurrentHealth = MaxHealth;

    public void AddDice(Dice dice) => DiceInventory.AddDice(dice);
    public IEnumerable<Dice> GetWorkingDices() => DiceInventory.GetWorkingDices();
    public IEnumerable<Dice> GetBrokenDices() => DiceInventory.GetBrokenDices();
    public void AddUpgrade(Upgrade upgrade) => UpgradeHolder.AddChild(upgrade);
    public IEnumerable<Upgrade> GetUpgrades() => UpgradeHolder.GetChildren<Upgrade>();
    public void AddWeapon(WeaponBase weapon) => WeaponInv.AddWeapon(weapon);
    public IEnumerable<WeaponBase> GetWeapons() => WeaponInv.GetChildren<WeaponBase>();
}
