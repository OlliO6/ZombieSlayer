using System;
using Additions;
using Godot;

public class Bullet : Area2D, IKillable, IDamageDealer
{
    [Export] public int damage;
    [Export] public float maxLivetime = 3;
    [Export] private bool goTroughAllTargets = false;
    [Export] public int goThroughTargetsCount = 0;

    public bool IsDead { get; set; }
    public float speed;

    public TimeAwaiter liveAwaiter;

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    public virtual int DamageAmount => damage;

    public override void _Ready()
    {
        liveAwaiter = new TimeAwaiter(this, maxLivetime, onCompleted: DieInstant);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (IsDead) return;
        GlobalPosition += speed * delta * GlobalTransform.x.Normalized();
    }

    [TroughtEditor]
    protected virtual void OnBodyEntered(Node body)
    {
        Die();
    }
    public virtual void DieInstant()
    {
        IsDead = true;
        QueueFree();
    }
    public virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        AnimationPlayer.Play("Die");
    }

    public bool AllowDamageTo(IDamageable to)
    {
        if (IsDead) return false;

        HitTarget();
        return true;
    }

    public virtual void HitTarget()
    {
        goThroughTargetsCount--;
        if (!goTroughAllTargets && goThroughTargetsCount <= 0)
        {
            Die();
        }
    }
}
