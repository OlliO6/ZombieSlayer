using Godot;
using System;

public class DamageUpgrade : Upgrade
{
    const int amount = 1;

    public new static string GetDescription() => "Deal more damage";

    public override void AddBuff(Player player)
    {
        player.extraDamage += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.extraDamage -= amount;
    }
}
