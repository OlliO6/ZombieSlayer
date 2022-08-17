using System;
using Additions;
using Godot;

public class WeaponBase : Node2D
{
    [Signal] public delegate void Attacked();

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
        InputManager.instance.Connect(nameof(InputManager.AttackInputStarted), this, nameof(OnAttackInputStarted));
        InputManager.instance.Connect(nameof(InputManager.AttackInputEnded), this, nameof(OnAttackInputEnded));
        AnimationPlayer.Connect("animation_finished", this, nameof(AnimationFinished));
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
        if (InputManager.attackInput && !disabled) AttackInputProcess();
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
    protected virtual void AttackInputProcess() { }
    public virtual void Attack() { isAttacking = true; EmitSignal(nameof(Attacked)); }
    protected virtual void AnimationFinished(string animation)
    {
        if (animation is "Attack")
        {
            isAttacking = false;
            AttackFinished();
        }
    }
    protected virtual void AttackFinished() { }
}
