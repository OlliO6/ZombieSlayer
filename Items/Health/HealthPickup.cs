using Godot;
using System;

public class HealthPickup : PickupBase
{
    [Export] private int amount = 1;

    public override void Collect()
    {
        Player.currentPlayer.CurrentHealth += amount;
    }

    public override bool IsCollectable()
    {
        Player player = Player.currentPlayer;

        return player.CurrentHealth + amount <= player.MaxHealth;
    }
}
