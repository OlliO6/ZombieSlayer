using Godot;
using System.Threading;

public class DamagingHurtArea : HurtArea, IDamageDealer
{
    [Signal] public delegate void DealingDamage(Node from, Node to);
    [Signal] public delegate void DamageDealed(Node to);
    [Export] public int DamageAmount { get; set; }

    private CancellationTokenSource dealDamageCancellation;

    public void CancelDamage()
    {
        dealDamageCancellation.Cancel();
    }

    bool IDamageDealer.AllowDamageTo(IDamageable to)
    {
        dealDamageCancellation = new();

        EmitSignal(nameof(DealingDamage), this, to as Node);

        if (dealDamageCancellation.Token.IsCancellationRequested) return false;

        EmitSignal(nameof(DamageDealed));

        return true;
    }
}
