using System.Collections.Generic;
using Additions;
using Godot;

public class LoadGun : ShootingWeaponBase
{
    [Signal] public delegate void LoadStarted();

    public Curve powerOverLoad, spreadOverLoad;
    public Curve bulletDamageOverPower, bulletSpeedOverPower, bulletScaleOverPower;
    public float bulletLivetime, bulletPowerLossPerHit;
    public float loadTime;

    private bool isLoading;
    private float loadProgress;
    private PlayerCamShakeInducer storerForPlayerCamShakeInducer;

    public PlayerCamShakeInducer PlayerCamShakeInducer => this.LazyGetNode(ref storerForPlayerCamShakeInducer, "PlayerCamShakeInducer");
    public float CurrentPower { get; private set; }
    public float LoadProgress
    {
        get => loadProgress;
        set
        {
            loadProgress = value;
            CurrentPower = isLoading ? powerOverLoad.InterpolateBaked(value) : 0;
        }
    }

    protected override void ApplyData()
    {
        base.ApplyData();

        loadTime = data.Get<float>("LoadTime");
        bulletLivetime = bulletData.Get<float>("Livetime");
        bulletPowerLossPerHit = bulletData.Get<float>("PowerLossPerHit");

        powerOverLoad = data.Get<Curve>("PowerOverLoadCurve");
        spreadOverLoad = data.Get<Curve>("SpreadOverLoadCurve");
        bulletDamageOverPower = bulletData.Get<Curve>("DamageOverPowerCurve");
        bulletSpeedOverPower = bulletData.Get<Curve>("SpeedOverPowerCurve");
        bulletScaleOverPower = bulletData.Get<Curve>("ScaleOverPowerCurve");
    }

    public override float GetBulletLivetime() => bulletLivetime;
    public override float GetSpread() => spreadOverLoad.InterpolateBaked(LoadProgress);

    public override void Attack()
    {
        if (!IsInstanceValid(lastBullet)) return;

        isAttacking = true;

        lastBullet.dead = false;
        lastBullet.liveAwaiter.Continue();
        InstantiatePoint.RemoveChild(lastBullet);
        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: GetSpread())));

        if (lastBullet is LoadBullet loadBullet)
        {
            loadBullet.Power = CurrentPower;
            loadBullet.powerLoosePerHit = bulletPowerLossPerHit;
        }
        lastBullet.maxLivetime = GetBulletLivetime();

        PlayerCamShakeInducer.Shake(CurrentPower);
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
            LoadProgress = 0;
            lastBullet = bulletScene.Instance<Bullet>();
            if (lastBullet is LoadBullet loadBullet)
            {
                loadBullet.damageOverPower = bulletDamageOverPower;
                loadBullet.speedOverPower = bulletSpeedOverPower;
                loadBullet.scaleOverPower = bulletScaleOverPower;
            }
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

        LoadProgress = (LoadProgress + delta / loadTime).Clamp01();
        (lastBullet as LoadBullet).Power = CurrentPower;
    }

    private void PauseBullet() => lastBullet.liveAwaiter.Pause();

    protected override void AttackInputEnded()
    {
        if (isAttacking || !isLoading) return;

        isLoading = false;
        if (loadProgress > 0)
        {
            Attack();
            return;
        }
        lastBullet?.QueueFree();
    }
}
