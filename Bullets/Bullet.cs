using Godot;
using System;
using Additions;

public class Bullet : Area2D, IKillable, IDamageDealer
{
    [Export] public int DamageAmount { get; set; }
    [Export] public float maxLivetime = 3;

    public float speed;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    protected bool dead;

    public override void _Ready()
    {
        Timer timer = new Timer();
        this.AddChild(timer);
        timer.Connect("timeout", this, nameof(DieInstant));
        timer.Start(maxLivetime);
    }



    public override void _PhysicsProcess(float delta)
    {
        if (dead) return;
        Position += speed * delta * Transform.x;
    }

    [TroughtSignal]
    protected virtual void OnBodyEntered(Node body)
    {
        Die();
    }
    public virtual void DieInstant()
    {
        dead = true;
        QueueFree();
    }
    public virtual void Die()
    {
        dead = true;
        AnimationPlayer.Play("Die");
    }

    public virtual void DamageReceived(IDamageable to)
    {
        Die();
    }
}
