using Godot;
using System;
using System.Collections.Generic;

public class DiceContainer : VBoxContainer
{
    [Export] private PackedScene diceFieldScene;

    [TroughtSignal]
    private void OnOpenMenuStarted() => UpdateDices();

    private void UpdateDices()
    {
        if (Player.currentPlayer is null) return;

        IEnumerable<Dice> dices = Player.currentPlayer.GetWorkingDices();

        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        foreach (Dice dice in dices)
        {
            DiceField diceField = diceFieldScene.Instance<DiceField>();

            diceField.dice = dice;

            AddChild(diceField);
        }
    }

    public void ShowDiceScenes()
    {

    }
}
