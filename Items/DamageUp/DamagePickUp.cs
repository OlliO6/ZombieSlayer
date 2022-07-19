using Godot;
using System;

public class DamagePickUp : PickupBase
{
    [Export] public int amount = 1;

    public override void Collect()
    {
        Player.currentPlayer.AddUpgrade(new DamageUpgrade(amount));
    }
}
