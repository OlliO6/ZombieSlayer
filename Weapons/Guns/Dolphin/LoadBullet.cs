using System.Collections.Generic;
using Additions;
using Godot;

public class LoadBullet : Bullet, IKillable, IDamageDealer
{
    [Export(PropertyHint.Range, "0,1")] private float power = 1;
    [Export(PropertyHint.Range, "0,1")] private float powerLoosePerHit = 1, minPower = 0;
    [Export] private float dieAnimSpeed;
    [Export] public Curve damageOverPower, speedOverPower, scaleOverPower;

    public override int DamageAmount => (damage * damageOverPower.InterpolateBaked(Power)).Round();
    public float Speed => speed * speedOverPower.InterpolateBaked(Power);

    public float Power
    {
        get => power;
        set
        {
            power = value;
            if (value < minPower)
            {
                Die();
                return;
            }
            Scale = Vector2.One * scaleOverPower.InterpolateBaked(value);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (dead) return;
        GlobalPosition += Speed * delta * GlobalTransform.x.Normalized();
    }

    public override void HitTarget()
    {
        SetDeferred(nameof(Power), Power - powerLoosePerHit);
    }

    public override void Die()
    {
        if (dead) return;
        dead = true;
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, Scale.x / dieAnimSpeed);
        ToSignal(tween, "finished").OnCompleted(DieInstant);
    }
}
