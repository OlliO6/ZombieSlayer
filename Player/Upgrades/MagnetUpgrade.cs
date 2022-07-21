using Godot;
using System;

public class MagnetUpgrade : Upgrade
{
    const float amount = 16;

    public new static string GetDescription() => "Attract coins and other collectables";

    public override void AddBuff(Player player)
    {
        player.MagnetAreaSize += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.MagnetAreaSize -= amount;
    }
}
