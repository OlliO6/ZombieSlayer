using Godot;

public class UpgradePickup : PickupBase
{
    [Export] private CSharpScript upgradeType;
    public override void Collect()
    {
        Upgrade upgrade = upgradeType.New() as Upgrade;
        upgrade.Name = upgrade.GetType().ToString();
        Player.currentPlayer.AddUpgrade(upgrade);
    }
}