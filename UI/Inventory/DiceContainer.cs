using Godot;
using System.Collections.Generic;
using System.Linq;
using Additions;
using System;

public class DiceContainer : VBoxContainer
{
    [Export] private PackedScene diceFieldScene;

    [TroughtSignal]
    private void OnInventoryOpened() => UpdateDices();

    private void UpdateDices()
    {
        if (Player.currentPlayer is null) return;

        IEnumerable<Dice> dices = Player.currentPlayer.GetWorkingDices().Concat(Player.currentPlayer.GetBrokenDices());

        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        foreach (Dice dice in dices)
        {
            DiceField diceField = diceFieldScene.Instance<DiceField>();

            diceField.watchable = false;
            diceField.Watched = false;
            diceField.Selected = false;
            diceField.dice = dice;

            if (Owner is Inventory inventory)
            {
                diceField.Connect(nameof(DiceField.OnSelected), inventory, nameof(Inventory.SelectionChanged), new(true));
                diceField.Connect(nameof(DiceField.OnDeselected), inventory, nameof(Inventory.SelectionChanged), new(false));
            }

            AddChild(diceField);
        }
        Update();
    }
}
