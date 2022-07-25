using Godot;
using System;

public abstract class Upgrade : Node
{
    public static string GetDescription() => "";

    public override void _EnterTree()
    {
        if (Player.currentPlayer is not null) AddBuff(Player.currentPlayer);
    }

    public override void _ExitTree()
    {
        if (Player.currentPlayer is not null) RemoveBuff(Player.currentPlayer);
    }

    public abstract void AddBuff(Player player);
    public abstract void RemoveBuff(Player player);
}
