using Godot;
using System;

public class InteractionPickupBase : Area2D
{
    [Signal] public delegate void OnCollected();
    [Signal] public delegate void OnPlayerEntered();
    [Signal] public delegate void OnPlayerExited();

    protected bool playerInArea;
    protected bool collected;

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (collected || Player.currentPlayer is null || playerInArea) return;

        if (!IsCollectable()) return;

        playerInArea = true;
        GD.Print("OnPlayerEntered");
        EmitSignal(nameof(OnPlayerEntered));
    }

    [TroughtSignal]
    private void OnAreaExited(Area2D area)
    {
        if (collected || !playerInArea) return;

        playerInArea = false;
        GD.Print("OnPlayerExited");
        EmitSignal(nameof(OnPlayerExited));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (collected) return;

        if (playerInArea && IsCollectable() && @event.IsActionPressed("Interact"))
        {
            collected = true;
            Collect();
            GD.Print("OnCollected");
            EmitSignal(nameof(OnCollected));
        }
    }

    public virtual void Collect() { }
    public virtual bool IsCollectable() => true;
}
