using Additions;
using Godot;
using System.Linq;
using System.Collections.Generic;

public class DiceStats : Control
{
    [Export] private PackedScene diceSceneFieldScene;

    #region DiceScenesContainer Reference

    private GridContainer storerForDiceScenesContainer;
    public GridContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, _DiceScenesContainer);
    [Export] private NodePath _DiceScenesContainer = "DiceScenesContainer";

    #endregion
    #region TypeLabel Reference

    private Label storerForTypeLabel;
    public Label TypeLabel => this.LazyGetNode(ref storerForTypeLabel, _TypeLabel);
    [Export] private NodePath _TypeLabel = "TypeLabel";

    #endregion
    #region RepairCostLabel Reference

    private Label storerForRepairCostLabel;
    public Label RepairCostLabel => this.LazyGetNode(ref storerForRepairCostLabel, _RepairCostLabel);
    [Export] private NodePath _RepairCostLabel = "RepairCostLabel";

    #endregion
    #region RepairButton Reference

    private Button storerForRepairButton;
    public Button RepairButton => this.LazyGetNode(ref storerForRepairButton, _RepairButton);
    [Export] private NodePath _RepairButton = "RepairButton";

    #endregion

    public void ShowStats(Dice dice)
    {
        Show();
        ShowDiceScenes(dice);

        TypeLabel.Text = $"{(dice.broken ? "Broken " : "")}{dice.Filename.GetFile().BaseName()}";

        int repairCost = dice.GetRepairCost();

        RepairCostLabel.Text = repairCost.ToString();

        RepairCostLabel.Visible = dice.broken;
        RepairButton.Visible = dice.broken;
        RepairButton.Disabled = repairCost > (Player.currentPlayer is null ? 0 : Player.currentPlayer.Coins);
    }


    private void ShowDiceScenes(Dice dice)
    {
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
