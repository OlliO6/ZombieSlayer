using System;
using Additions;
using Godot;

public class WeaponBase : Node2D
{
    [Signal] public delegate void AttackStarted();
    [Signal] public delegate void AttackFinished();

    [Export(PropertyHint.File, "*tscn,*scn")] public string weaponPickup;
    [Export] public Texture icon;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    public bool disabled;

    public bool isAttacking;

    public override void _Ready()
    {
        InputManager.AttackInputStarted += OnAttackInputStarted;
        InputManager.AttackInputEnded += OnAttackInputEnded;
        AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
    }

    public virtual void Disable()
    {
        Visible = false;
        disabled = true;
    }
    public virtual void Enable()
    {
        Visible = true;
        disabled = false;
    }

    public override void _Process(float delta)
    {
        if (InputManager.attackInput && !disabled) AttackInputProcess(delta);
    }

    [TroughtSignal]
    private void OnAttackInputStarted()
    {
        if (!disabled) AttackInputStarted();
    }
    [TroughtSignal]
    private void OnAttackInputEnded()
    {
        if (!disabled) AttackInputEnded();
    }

    protected virtual void AttackInputStarted() { }
    protected virtual void AttackInputEnded() { }
    protected virtual void AttackInputProcess(float delta) { }
    public virtual void Attack() { isAttacking = true; EmitSignal(nameof(AttackStarted)); }
    protected virtual void OnAnimationFinished(string animation)
    {
        if (animation is "Attack")
        {
            isAttacking = false;
            OnAttackFinished();
        }
    }
    protected virtual void OnAttackFinished() { EmitSignal(nameof(AttackFinished)); }
}
