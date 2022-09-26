using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class DiceContainer : VBoxContainer
{
    [Export] private PackedScene diceFieldScene;

    [TroughtEditor]
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
            diceField.IsWatched = false;
            diceField.IsSelected = false;
            diceField.dice = dice;

            if (Owner is Inventory inventory)
            {
                diceField.Connect(nameof(DiceField.Selected), inventory, nameof(Inventory.FieldSelected), new(true));
                diceField.Connect(nameof(DiceField.Deselected), inventory, nameof(Inventory.FieldSelected), new(false));
            }

            AddChild(diceField);
        }
        Update();
    }
}
