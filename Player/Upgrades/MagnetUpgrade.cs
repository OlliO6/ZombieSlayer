using Godot;
using System;

public class MagnetUpgrade : Upgrade
{
    [Export] public float amount = 5;

    public MagnetUpgrade(float amount)
    {
        this.amount = amount;
    }
    public MagnetUpgrade() { }

    public override void AddBuff(Player player)
    {
        player.MagnetAreaSize += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.MagnetAreaSize -= amount;
    }
}
