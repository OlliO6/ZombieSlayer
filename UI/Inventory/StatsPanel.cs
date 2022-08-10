using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

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

    public void OnRepairDiceClicked()
    {
        if (Inventory is null || Player.currentPlayer is null) return;

        Dice dice = (Inventory.Selection as DiceField).dice;

        if (dice is null) return;
        int cost = dice.GetRepairCost();
        if (Player.currentPlayer.Coins < cost) return;

        Player.currentPlayer.Coins -= cost;

        dice.broken = false;

        Player.currentPlayer.AddDice(dice);

        Inventory.EmitSignal(nameof(Inventory.Opened));

        Inventory.CoinLabel.Text = Player.currentPlayer.Coins.ToString();

        IEnumerable<DiceField> matchingContainers = Inventory.DiceContainer.GetChildren<DiceField>().Where((DiceField diceField) => diceField.dice == dice);

        foreach (DiceField diceField in matchingContainers)
        {
            diceField.IsSelected = true;
        }
    }

    public void OnSellDiceClicked()
    {
        if (Inventory is null || Player.currentPlayer is null) return;

        Dice dice = (Inventory.Selection as DiceField).dice;

        int price = dice.GetSellPrice();

        if (dice is null) return;

        Player.currentPlayer.Coins += price;
        dice.GetParent().RemoveChild(dice);
        dice.QueueFree();

        Inventory.EmitSignal(nameof(Inventory.Opened));
        Inventory.CoinLabel.Text = Player.currentPlayer.Coins.ToString();
        Inventory.Selection = null;
    }
}
