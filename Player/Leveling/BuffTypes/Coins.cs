namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class Coins : LevelBuff
{
    [Export] public int coins = 10;

    public override void Apply()
    {
        Player.currentPlayer.Coins += coins;
    }

    public override string GetBuffText() => $"Got {coins} Coins";
}
