using Godot;
using System.Collections.Generic;

public class UpgradeContainer : GridContainer
{
    [TroughtSignal]
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
