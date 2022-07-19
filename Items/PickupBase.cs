using Godot;
using System;

public class PickupBase : Area2D
{
    [Signal] public delegate void OnCollected();

    protected bool collected;


    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (collected || Player.currentPlayer is null) return;

        if (!IsCollectable()) return;

        collected = true;
        Collect();
        EmitSignal(nameof(OnCollected));
    }

    public virtual void Collect() { }
    public virtual bool IsCollectable() => true;
    public virtual bool IsAttractable() => IsCollectable();
}
