using Godot;
using System;

public class SpeedPickUp : PickupBase
{
    [Export] public float amount = 5;

    public override void Collect()
    {
        Player.currentPlayer.AddUpgrade(new MovementSpeedUpgrade(amount));
    }
}
