using Godot;
using System;

public class DamageUpgrade : Upgrade
{
    [Export] public int amount = 1;

    public DamageUpgrade(int amount)
    {
        this.amount = amount;
    }
    public DamageUpgrade() { }

    public override void AddBuff(Player player)
    {
        player.extraDamage += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.extraDamage -= amount;
    }
}
