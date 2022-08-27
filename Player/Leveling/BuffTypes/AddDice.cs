namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class AddDice : LevelBuff
{
    [Export] private PackedScene diceScene;
    [Export] private PackedScene[] diceScenes = new PackedScene[6];

    public override void Apply()
    {
        Dice dice = diceScene.Instance<Dice>();
        dice.scenes = diceScenes;

        Player.currentPlayer.AddDice(dice);
    }

    public override string GetBuffText() => "Got a dice";
}
