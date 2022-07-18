using Godot;

public class MagnetPickup : PickupBase
{
    [Export] private float amount = 4;
    public override void Collect()
    {
        Player.currentPlayer.MagnetAreaSize += amount;
    }
}