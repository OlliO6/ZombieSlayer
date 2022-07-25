using Godot;

public class UpgradePickup : PickupBase
{
    [Export] private CSharpScript upgradeType;
    public override void Collect()
    {
        Player.currentPlayer.AddUpgrade(upgradeType.New() as Upgrade);
    }
}