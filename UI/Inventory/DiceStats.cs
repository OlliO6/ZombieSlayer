using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class DiceStats : Control
{
    [Export] private PackedScene diceSceneFieldScene;

    #region DiceScenesContainer Reference

    private DiceScenesContainer storerForDiceScenesContainer;
    public DiceScenesContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, "%DiceScenesContainer");

    #endregion
    #region TypeLabel Reference

    private Label storerForTypeLabel;
    public Label TypeLabel => this.LazyGetNode(ref storerForTypeLabel, "%TypeLabel");

    #endregion
    #region RepairButton Reference

    private Button storerForRepairButton;
    public Button RepairButton => this.LazyGetNode(ref storerForRepairButton, "%RepairButton");

    #endregion
    #region SellButton Reference

    private Button storerForSellButton;
    public Button SellButton => this.LazyGetNode(ref storerForSellButton, "%SellButton");

    #endregion

    public override void _Ready()
    {
        if (GetParent() is StatsPanel statsPanel)
        {
            RepairButton.Connect("pressed", statsPanel, nameof(StatsPanel.OnRepairDiceClicked));
            SellButton.Connect("pressed", statsPanel, nameof(StatsPanel.OnSellDiceClicked));
        }
    }

    public void ShowStats(Dice dice)
    {
        Show();
        DiceScenesContainer.Scenes = dice.scenes.ToList();

        TypeLabel.Text = $"{(dice.broken ? "Broken " : "")}{dice.Filename.GetFile().BaseName()}";

        int repairCost = dice.GetRepairCost();
        int sellPrice = dice.GetSellPrice();

        RepairButton.Text = $"Repair {repairCost}";
        RepairButton.Visible = dice.broken;
        RepairButton.Disabled = repairCost > (Player.currentPlayer is null ? 0 : Player.currentPlayer.Coins);

        SellButton.Text = $"Sell {sellPrice}";
    }
}
