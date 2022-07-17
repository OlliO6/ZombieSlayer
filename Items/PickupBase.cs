using Godot;
using System;

public class PickupBase : Area2D
{
    [Signal] public delegate void OnCollected();


    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        Collect();

        EmitSignal(nameof(OnCollected));
    }

    public virtual void Collect()
    {

    }
}
