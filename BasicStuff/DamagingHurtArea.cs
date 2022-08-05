using System;
using System.Threading;
using Additions;
using Godot;

public class DamagingHurtArea : HurtArea, IDamageDealer
{
    [Signal] public delegate void DamageDealed(Node to);
    [Export] public int DamageAmount { get; set; }
    public Func<IDamageable, bool> AllowDealingDamage = (d) => true;

    bool IDamageDealer.AllowDamageTo(IDamageable to)
    {
        if (!AllowDealingDamage(to)) return false;

        EmitSignal(nameof(DamageDealed));
        return true;
    }
}
