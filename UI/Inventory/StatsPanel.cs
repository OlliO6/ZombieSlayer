using Godot;
using System.Linq;
using System.Collections.Generic;
using Additions;

public class StatsPanel : Control
{
    #region DiceStats Reference

    private DiceStats storerForDiceStats;
    public DiceStats DiceStats => this.LazyGetNode(ref storerForDiceStats, "DiceStats");

    #endregion

    #region WeaponStats Reference

    private WeaponStats storerForWeaponStats;
    public WeaponStats WeaponStats => this.LazyGetNode(ref storerForWeaponStats, "WeaponStats");

    #endregion

    Inventory Inventory => Owner.LazyObjectCast(ref _inventory);
    Inventory _inventory;

    [TroughtSignal]
    private void OnInventorySelectionChanged(Node to) => ShowStats(to);

    public void ShowStats(Node from)
    {
        if (from is DiceField diceField)
        {
            ShowDiceStats(diceField.dice);
            return;
        }
        if (from is DragableWeaponField weaponField)
        {
            ShowWeaponStats(weaponField.weapon);
            return;
        }

        HideAllStats();
    }

    private void ShowWeaponStats(WeaponBase weapon)
    {
        DiceStats.Hide();
        WeaponStats.ShowStats(weapon);
    }

    private void ShowDiceStats(Dice dice)
    {
        DiceStats.ShowStats(dice);
        WeaponStats.Hide();
    }

    private void HideAllStats()
    {
        DiceStats.Hide();
        WeaponStats.Hide();
    }


    [TroughtSignal]
    private void OnRepairDiceClicked()
    {
        if (Inventory is null || Player.currentPlayer is null) return;

        Dice dice = (Inventory.Selection as DiceField).dice;

        int cost = dice.GetRepairCost();

        if (dice is null || Player.currentPlayer.Coins < cost) return;

        Player.currentPlayer.Coins -= cost;

        dice.broken = false;

        Player.currentPlayer.AddDice(dice);

        Inventory.EmitSignal(nameof(Inventory.OnOpened));

        Inventory.CoinLabel.Text = Player.currentPlayer.Coins.ToString();

        IEnumerable<DiceField> matchingContainers = Inventory.DiceContainer.GetChildren<DiceField>().Where((DiceField diceField) => diceField.dice == dice);

        foreach (DiceField diceField in matchingContainers)
        {
            diceField.Selected = true;
        }
    }
}
