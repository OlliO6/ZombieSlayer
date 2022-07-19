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
        if (collected || Player.currentPlayer is null) return;

        if (!IsCollectable()) return;

        playerInArea = true;
        EmitSignal(nameof(OnPlayerEntered));
    }

    [TroughtSignal]
    private void OnAreaExited(Area2D area)
    {
        if (collected) return;

        playerInArea = false;
        EmitSignal(nameof(OnPlayerExited));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (collected) return;

        if (playerInArea && IsCollectable() && @event.IsActionPressed("Interact"))
        {
            collected = true;
            Collect();
            EmitSignal(nameof(OnCollected));
        }
    }

    public virtual void Collect() { }
    public virtual bool IsCollectable() => true;
}
