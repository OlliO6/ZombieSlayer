using Godot;
using System;
using Additions;

public class Bullet : Area2D, IKillable, IDamageDealer
{
    [Export] public int DamageAmount { get; set; }

    public float speed;

    public override void _PhysicsProcess(float delta)
    {
        Position += speed * delta * Transform.x;
    }

    [TroughtSignal]
    private void OnBodyEntered(Node body)
    {
        Die();
    }

    public void Die()
    {
        QueueFree();
    }

    public void DamageReceived(IDamageable to)
    {
        Die();
    }
}
