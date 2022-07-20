using Godot;
using System;

public class MagnetUpgrade : Upgrade
{
    const float amount = 5;

    public override void AddBuff(Player player)
    {
        player.MagnetAreaSize += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.MagnetAreaSize -= amount;
    }
}
