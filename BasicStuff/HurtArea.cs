using Godot;
using System;
using Additions;

public class HurtArea : Area2D
{
    [Export] private bool damageFromAreas = true, damageFromBodies = false;

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

    private void ApplyDamage(IDamageDealer dealer)
    {
        DamageReceiver.GetDamage(dealer.DamageAmount);
        dealer.DamageReceived(DamageReceiver);
    }
}
