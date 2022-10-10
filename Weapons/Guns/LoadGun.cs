using System.Collections.Generic;
using Additions;
using Godot;

public class LoadGun : ShootingWeaponBase
{
    [Signal] public delegate void LoadStarted();

    [Export] public Curve spread, power;
    public float loadingTime, minProgress;

    bool isLoading;
    float loadProgress;

    private PlayerCamShakeInducer storerForPlayerCamShakeInducer;
    public PlayerCamShakeInducer PlayerCamShakeInducer => this.LazyGetNode(ref storerForPlayerCamShakeInducer, "PlayerCamShakeInducer");

    protected override void ApplyData()
    {
        base.ApplyData();
    }

    public override void Attack()
    {
        if (!IsInstanceValid(lastBullet)) return;
        isAttacking = true;
        lastBullet.dead = false;
        lastBullet.liveAwaiter.Continue();

        Vector2 globPos = lastBullet.GlobalPosition;
        InstantiatePoint.RemoveChild(lastBullet);
        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalPosition = globPos;

        lastBullet.speed = (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y);
        lastBullet.damage = GetBulletDamage();
        lastBullet.maxLivetime = bulletLivetime;
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: bulletSpread * spread.InterpolateBaked(loadProgress))));

        float _power = power.InterpolateBaked(loadProgress);

        if (lastBullet is LoadBullet loadBullet)
        {
            loadBullet.Power = _power;
        }

        PlayerCamShakeInducer.Shake(_power);
        AnimationPlayer.Stop();
        AnimationPlayer.Play("Shoot");

        EmitSignal(nameof(AttackStarted));
    }

    protected override void AttackInputProcess(float delta)
    {
        if (isAttacking) return;

        if (!isLoading)
        {
            isLoading = true;
            loadProgress = 0;
            lastBullet = bulletScene.Instance<Bullet>();
            lastBullet.dead = true;
            AnimationPlayer.Play("Load");
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

        (lastBullet as LoadBullet).Power = power.InterpolateBaked(loadProgress);
    }

    private void PauseBullet() => lastBullet.liveAwaiter.Pause();

    protected override void AttackInputEnded()
    {
        if (isAttacking || !isLoading) return;

        isLoading = false;
        if (loadProgress > minProgress)
        {
            Attack();
            return;
        }
        lastBullet?.QueueFree();
    }
}
