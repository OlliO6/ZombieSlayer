using Godot;
using System;

public class Coin : Area2D
{
    [Export] public int amount;

    [Signal] public delegate void OnCollected();

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        Player.currentPlayer.Coins += amount;

        EmitSignal(nameof(OnCollected));
    }
}
