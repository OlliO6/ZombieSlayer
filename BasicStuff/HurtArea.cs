using Godot;
using System;
using Additions;

public class HurtArea : Area2D
{
    #region DamageReceiver Reference

    private IDamageable storerForDamageReceiver;
    public IDamageable DamageReceiver => this.LazyGetNode(ref storerForDamageReceiver, _DamageReceiver);
    [Export] private NodePath _DamageReceiver = "..";

    #endregion

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        ApplyDamage(area);
    }

    private void ApplyDamage(Area2D area)
    {
        IDamageDealer dealer = area as IDamageDealer;

        DamageReceiver.GetDamage(dealer.DamageAmount);
        dealer.DamageReceived(DamageReceiver);
    }
}
