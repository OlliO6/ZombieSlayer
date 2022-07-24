using Godot;
using System;
using Additions;

public class StatsPanel : Control
{
    #region DiceStats Reference

    private Control storerForDiceStats;
    public Control DiceStats => this.LazyGetNode(ref storerForDiceStats, "DiceStats");

    #endregion

    #region WeaponStats Reference

    private Control storerForWeaponStats;
    public Control WeaponStats => this.LazyGetNode(ref storerForWeaponStats, "WeaponStats");

    #endregion

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
        WeaponStats.Show();
    }

    private void ShowDiceStats(Dice dice)
    {
        DiceStats.Show();
        WeaponStats.Hide();
    }

    private void HideAllStats()
    {
        DiceStats.Hide();
        WeaponStats.Hide();
    }
}
