using System.Threading;
using Additions;
using Godot;

public class HurtArea : Area2D
{
    [Export] private bool damageFromAreas = true, damageFromBodies = false;

    [Signal] public delegate void DamageApplied();

    #region DamageReceiver Reference

    private IDamageable storerForDamageReceiver;
    public IDamageable DamageReceiver => this.LazyGetNode(ref storerForDamageReceiver, _DamageReceiver);
    [Export] private NodePath _DamageReceiver = "..";

    #endregion

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (!damageFromAreas) return;

        if (area is IDamageDealer dealer) ApplyDamage(dealer);
    }

    [TroughtSignal]
    private void OnBodyEntered(Node body)
    {
        if (!damageFromBodies) return;

        if (body is IDamageDealer dealer) ApplyDamage(dealer);
    }

    public void Check()
    {
        if (damageFromAreas)
        {
            foreach (Area2D area in GetOverlappingAreas())
            {
                if (area is IDamageDealer dealer)
                {
                    ApplyDamage(dealer);
                    return;
                }
            }
        }
        if (damageFromBodies)
        {
            foreach (PhysicsBody2D body in GetOverlappingBodies())
            {
                if (body is IDamageDealer dealer)
                {
                    ApplyDamage(dealer);
                    return;
                }
            }
        }
    }

    private void ApplyDamage(IDamageDealer dealer)
    {
        if (!DamageReceiver.AllowDamageFrom(dealer) || !dealer.AllowDamageTo(DamageReceiver))
            return;

        DamageReceiver.GetDamage(dealer.DamageAmount);

        EmitSignal(nameof(DamageApplied));
    }
}
