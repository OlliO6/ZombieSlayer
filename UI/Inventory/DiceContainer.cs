using Godot;
using System.Collections.Generic;
using System.Linq;
using Additions;
using System;

public class DiceContainer : VBoxContainer
{
    [Export] private PackedScene diceFieldScene, diceSceneFieldScene;


    #region DiceScenesContainer Reference

    private GridContainer storerForDiceScenesContainer;
    public GridContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, _DiceScenesContainer);
    [Export] private NodePath _DiceScenesContainer = "DiceScenesContainer";

    #endregion

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

            // if (Owner is Inventory) diceField.Connect(nameof(DiceField.OnSelected), Owner, nameof(Inventory.SelectionChanged), new(diceField));

            AddChild(diceField);
        }

        ShowDiceScenes(null);
    }

    public void ShowDiceScenes(DiceField diceField)
    {
        Dice dice = diceField is null ? null : diceField.dice;

        foreach (DiceSceneField sceneField in DiceScenesContainer.GetChildren())
        {
            sceneField.QueueFree();
            DiceScenesContainer.RemoveChild(sceneField);
        }

        if (dice is null || dice.scenes is null) return;

        foreach (PackedScene scene in dice.scenes)
        {
            DiceSceneField sceneField = diceSceneFieldScene.Instance<DiceSceneField>();
            sceneField.Scene = scene;

            DiceScenesContainer.AddChild(sceneField);
        }
    }
}
