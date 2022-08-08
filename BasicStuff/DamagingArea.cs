using System;
using Godot;

public class DamagingArea : Area2D, IDamageDealer
{
    [Export] public int DamageAmount { get; set; }
    [Signal] public delegate void DamageDealed(Node to);

    public Func<IDamageable, bool> AllowDealingDamage = (d) => true;

    bool IDamageDealer.AllowDamageTo(IDamageable to)
    {
        if (!AllowDealingDamage(to)) return false;

        EmitSignal(nameof(DamageDealed));
        return true;
    }
}
