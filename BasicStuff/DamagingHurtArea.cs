using Godot;
using System;

public class DamagingHurtArea : HurtArea, IDamageDealer
{
    [Signal] public delegate void DamageDealed(Node to);
    [Export] public int DamageAmount { get; set; }

    public void DamageReceived(IDamageable to)
    {
        EmitSignal(nameof(DamageDealed), to as Node);
    }
}
