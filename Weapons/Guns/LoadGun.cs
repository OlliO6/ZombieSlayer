using System.Collections.Generic;
using Additions;
using Godot;

public class LoadGun : GunBase
{
    [Signal] public delegate void LoadStarted();
    [Export] private float loadingTime, minProgress = 0.25f;
    [Export] private Curve bulletSize, damage, speed, spread, livetime;

    bool isLoading;
    float loadProgress;


    public override void Attack()
    {
        isAttacking = true;
        lastBullet.dead = false;
        lastBullet.liveAwaiter.Continue();

        Debug.LogU(this, $@"Speed: {(float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y) * speed.Interpolate(loadProgress)}
Damage: {GetBulletDamageAmount()}
Scale: {Vector2.One * bulletSize.InterpolateBaked(loadProgress)}");

        Vector2 globPos = lastBullet.GlobalPosition;
        InstantiatePoint.RemoveChild(lastBullet);
        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalPosition = globPos;

        lastBullet.speed = (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y) * speed.Interpolate(loadProgress);
        lastBullet.DamageAmount = GetBulletDamageAmount();
        lastBullet.maxLivetime = bulletLivetime;
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: bulletSpread * spread.Interpolate(loadProgress))));
        lastBullet.Scale = Vector2.One * bulletSize.InterpolateBaked(loadProgress);

        AnimationPlayer.Stop();
        AnimationPlayer.Play("Shoot");

        EmitSignal(nameof(AttackStarted));
    }

    public override int GetBulletDamageAmount() => Mathf.RoundToInt(base.GetBulletDamageAmount() * damage.Interpolate(loadProgress));

    protected override void AttackInputProcess(float delta)
    {
        if (isAttacking) return;

        if (!isLoading)
        {
            isLoading = true;
            loadProgress = 0;
            lastBullet = bulletScene.Instance<Bullet>();
            lastBullet.dead = true;
            CallDeferred(nameof(PauseBullet));
            InstantiatePoint.AddChild(lastBullet);
            EmitSignal(nameof(LoadStarted));
        }

        if (!IsInstanceValid(lastBullet))
        {
            isLoading = false;
            return;
        }

        loadProgress += delta / loadingTime;
        loadProgress = loadProgress.Clamp01();

        lastBullet.Scale = Vector2.One * bulletSize.InterpolateBaked(loadProgress);
    }

    private void PauseBullet() => lastBullet.liveAwaiter.Pause();

    protected override void AttackInputEnded()
    {
        if (isAttacking) return;

        isLoading = false;
        if (loadProgress > minProgress)
        {
            Attack();
            return;
        }
        lastBullet?.QueueFree();
    }
}
