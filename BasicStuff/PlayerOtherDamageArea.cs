using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class PlayerOtherDamageArea : Area2D, IDamageDealer
{
    [Signal] public delegate void DamageDealed(Node to);

    [Export] public bool toPlayer = true;
    [Export] public int damageToPlayer = 1;
    [Export] public bool toOthers = true;
    [Export] public int damageToOthers = 10;

    public int DamageAmount { get; set; }

    public Func<IDamageable, bool> AllowDealingDamage = (_) => true;

    bool IDamageDealer.AllowDamageTo(IDamageable to)
    {
        if (!AllowDealingDamage(to)) return false;

        switch (to)
        {
            case Player:
                if (!toPlayer) return false;
                DamageAmount = damageToPlayer;
                break;
            default:
                if (!toOthers) return false;
                DamageAmount = damageToOthers;
                break;
        }

        EmitSignal(nameof(DamageDealed));
        return true;
    }
}
