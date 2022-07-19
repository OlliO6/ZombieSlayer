using Godot;
using System;
using Additions;

public class WeaponBase : Node2D
{
    [Export(PropertyHint.File, "*tscn,*scn")] public string weaponPickup;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    public bool disabled;
    protected bool attackInput;

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

    public override void _Input(InputEvent @event)
    {
        if (disabled || !@event.IsAction("Attack")) return;

        if (@event.IsPressed())
        {
            attackInput = true;
            AttackInputStarted();
            return;
        }
        attackInput = false;
        AttackInputEnded();
    }

    public override void _Process(float delta)
    {
        if (attackInput && !disabled) AttackInputProcess();
    }

    protected virtual void AttackInputStarted() { }
    protected virtual void AttackInputEnded() { }
    protected virtual void AttackInputProcess() { }
    public virtual void Attack() { }
}
