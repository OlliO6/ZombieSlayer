using System;
using Additions;
using Godot;

public class Bullet : Area2D, IKillable, IDamageDealer
{
    [Export] public int DamageAmount { get; set; }
    [Export] public float maxLivetime = 3;
    [Export] private bool goTroughAllTargets = false;
    [Export] public int goThroughTargetsCount = 0;

    public float speed;

    #region AnimationPlayer Reference

    private AnimationPlayer storerForAnimationPlayer;
    public AnimationPlayer AnimationPlayer => this.LazyGetNode(ref storerForAnimationPlayer, "AnimationPlayer");

    #endregion

    protected bool dead;

    public override void _Ready()
    {
        new TimeAwaiter(this, maxLivetime, onCompleted: DieInstant);
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
