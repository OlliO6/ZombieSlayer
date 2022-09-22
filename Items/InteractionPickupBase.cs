using Additions;
using Godot;

public class InteractionPickupBase : Area2D, IInteractable
{
    [Signal] public delegate void Collected();
    [Signal] public delegate void PlayerEntered();
    [Signal] public delegate void PlayerExited();

    protected bool collected;

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (collected || Player.currentPlayer is null) return;
        if (!IsCollectable()) return;
        Player.currentPlayer.AddInteractable(this);
    }

    [TroughtSignal]
    private void OnAreaExited(Area2D area)
    {
        if (collected) return;
        Player.currentPlayer.RemoveInteractable(this);
    }

    public void Interact()
    {
        if (collected || !IsCollectable()) return;

        Player.currentPlayer.RemoveInteractable(this);
        collected = true;
        Collect();
        EmitSignal(nameof(Collected));
        Debug.LogU(this, "Collected");
    }

    public virtual void Collect() { }
    public virtual bool IsCollectable() => true;

    public void Select()
    {
        EmitSignal(nameof(PlayerEntered));
    }

    public void Deselect()
    {
        EmitSignal(nameof(PlayerExited));
    }
}
