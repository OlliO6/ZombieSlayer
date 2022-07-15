using Godot;
using System;
using Additions;

public class Bullet : Area2D
{
    [TroughtSignal]
    private void OnBodyEntered(Node body)
    {
        Die();
    }

    [TroughtSignal]
    private void OnAreaEntered(Area2D body)
    {
        Die();
    }

    public void Die()
    {
        QueueFree();
    }
}
