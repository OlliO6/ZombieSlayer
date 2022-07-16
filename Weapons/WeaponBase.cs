using Godot;
using System;
using Additions;

public class WeaponBase : Node
{
    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    protected bool attackInput;

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsAction("Attack")) return;

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
        if (attackInput) AttackInputProcess();
    }

    protected virtual void AttackInputStarted() { }
    protected virtual void AttackInputEnded() { }
    protected virtual void AttackInputProcess() { }
    public virtual void Attack() { }
}
