using Additions;
using Godot;
using System;

public class Player : KinematicBody2D, IDamageable, IKillable, IHealth
{
    public static Player currentPlayer;
    [Export] public float movementSpeed, invincibilityTime;
    [Export] public int startCoins = 0;

    [Export] public int MaxHealth { get; set; }

    public int CurrentHealth
    {
        get => storerForCurrentHealth;
        set
        {
            GD.Print($"CurrentHealth: {value}; MaxHealth: {MaxHealth};");
            storerForCurrentHealth = value;
            EmitSignal(nameof(OnHealthChanged));
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
            EmitSignal(nameof(OnCoinsAmountChanged), value);
        }
    }
    private int coins;

    [Signal] public delegate void OnCoinsAmountChanged(int amount);
    [Signal] public delegate void OnInvincibilityStarted();
    [Signal] public delegate void OnInvincibilityEnded();
    [Signal] public delegate void OnHealthChanged();


    #region AnimationTree Reference

    private AnimationTree storerForAnimationTree;
    public AnimationTree AnimationTree => this.LazyGetNode(ref storerForAnimationTree, "AnimationTree");

    #endregion

    #region Sprite Reference

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    #endregion

    public bool isInvincible;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
        currentPlayer = this;
        Coins = startCoins;
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
        EmitSignal(nameof(OnInvincibilityStarted));
        Sprite.SetShaderParam("blinking", true);

        ToSignal(GetTree().CreateTimer(invincibilityTime), Constants.timeout).OnCompleted(() =>
        {
            isInvincible = false;
            EmitSignal(nameof(OnInvincibilityEnded));
            Sprite.SetShaderParam("blinking", false);
        });

        CurrentHealth -= amount;
    }

    public void Die()
    {
        SceneManager.LoadMenu();
    }

    public void AddDice(Dice dice)
    {
        GetNode("DiceHolder").AddChild(dice);
    }
}
