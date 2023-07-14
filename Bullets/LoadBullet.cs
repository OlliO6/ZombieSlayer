using System.Collections.Generic;
using Additions;
using Godot;

public class LoadBullet : Bullet
{
    [Export(PropertyHint.Range, "0,1")] public float powerLoosePerHit = 1;
    [Export] public float dieAnimSpeed;
    [Export] public Curve damageOverPower, speedOverPower, scaleOverPower;

    private float power = 1;

    public float Power
    {
        get => power;
        set
        {
            power = value;
            if (value < 0)
            {
                Die();
                return;
            }
            Scale = Vector2.One * scaleOverPower.InterpolateBaked(value);
            speed = speedOverPower.InterpolateBaked(value);
            damage = damageOverPower.InterpolateBaked(value).Round();
        }
    }

    public override void HitTarget()
    {
        SetDeferred(nameof(Power), Power - powerLoosePerHit);
    }

    public override void Die()
    {
        if (IsDead) return;
        IsDead = true;
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, Scale.x / dieAnimSpeed);
        ToSignal(tween, "finished").OnCompleted(DieInstant);
    }
}
