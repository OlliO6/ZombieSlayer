using Godot;
using System;

public class DamageUpgrade : Upgrade
{
    const float amount = 0.42f;

    public new static string GetDescription() => "Deal more damage";

    public override void AddBuff(Player player)
    {
        player.damageMultiplier += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.damageMultiplier -= amount;
    }
}
