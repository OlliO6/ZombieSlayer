using Godot;
using System;

public class MovementSpeedUpgrade : Upgrade
{
    [Export] public float amount = 5;

    public MovementSpeedUpgrade(float amount)
    {
        this.amount = amount;
    }
    public MovementSpeedUpgrade() { }

    public override void AddBuff(Player player)
    {
        player.movementSpeed += amount;
    }

    public override void RemoveBuff(Player player)
    {
        player.movementSpeed -= amount;
    }
}
