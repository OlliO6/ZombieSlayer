namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class Coins : LevelBuff
{
    public int coins;

    public override void Apply()
    {
        Player.currentPlayer.Coins += coins;
    }

    public override string GetBuffText() => $"Got {coins} Coins";
}
