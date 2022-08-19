using System;
using Additions;
using Godot;

public class Bullet : Area2D, IKillable, IDamageDealer
{
    [Export] public int DamageAmount { get; set; }
    [Export] public float maxLivetime = 3;
    [Export] private bool goTroughAllTargets = false;
    [Export] public int goThroughTargetsCount = 0;

    public bool dead;
    public float speed;

    public TimeAwaiter liveAwaiter;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    public override void _Ready()
    {
        liveAwaiter = new TimeAwaiter(this, maxLivetime, onCompleted: DieInstant);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead) return;
        GlobalPosition += speed * delta * (GlobalTransform.x).Normalized();
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

    public bool AllowDamageTo(IDamageable to)
    {
        if (dead) return false;

        if (!goTroughAllTargets && goThroughTargetsCount <= 0)
        {
            Die();
            return true;
        }

        goThroughTargetsCount--;

        return !dead;
    }
}
