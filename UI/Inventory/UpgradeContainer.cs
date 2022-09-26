using System.Collections.Generic;
using Godot;

public class UpgradeContainer : GridContainer
{
    [TroughtEditor]
    private void OnInventoryOpened() => UpdateAmounts();

    private void UpdateAmounts()
    {
        if (Player.currentPlayer is null) return;

        IEnumerable<Upgrade> upgrades = Player.currentPlayer.GetUpgrades();

        foreach (UpgradeDisplay upgradeDisplay in GetChildren())
        {
            int amount = 0;

            foreach (Upgrade upgrade in upgrades)
            {
                if (upgradeDisplay.upgradeType.InstanceHas(upgrade))
                {
                    amount++;
                }
            }

            upgradeDisplay.SetAmount(amount);
        }
    }
}
