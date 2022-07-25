using Godot;
using System;

public class MovementSpeedUpgrade : Upgrade
{
    const float amount = 5;

    public new static string GetDescription() => "Move faster";

    public override void AddBuff(Player player)
    {
        player.movementSpeed += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.movementSpeed -= amount;
    }
}
