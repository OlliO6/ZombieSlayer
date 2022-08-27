namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class Heal : LevelBuff
{
    public override void Apply()
    {
        Debug.LogU(this, "Healed");
        Player.currentPlayer?.Heal();
    }

    public override string GetBuffText() => "Healed Player";
}
