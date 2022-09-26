using Additions;
using Godot;

public class PickupBase : Area2D
{
    [Signal] public delegate void Collected();

    protected bool collected;


    [TroughtEditor]
    private void OnAreaEntered(Area2D area)
    {
        if (collected || Player.currentPlayer is null) return;

        if (!IsCollectable()) return;

        collected = true;
        Collect();
        EmitSignal(nameof(Collected));

        Debug.Log(this, "Collected");
    }

    public virtual void Collect() { }
    public virtual bool IsCollectable() => true;
    public virtual bool IsAttractable() => IsCollectable();
}
