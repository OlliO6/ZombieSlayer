using Additions;
using Godot;

public class Gun : ShootingWeaponBase
{
    public bool allowHold;
    public Vector2 bulletSpeedRange;
    public float bulletSpread, bulletLivetime;
    public int bulletDamage;

    protected override void ApplyData()
    {
        base.ApplyData();
        allowHold = data.Get<bool>("AllowHold");
        bulletDamage = (int)bulletData.Get<float>("Damage");
        bulletSpread = bulletData.Get<float>("Spread");
        bulletLivetime = bulletData.Get<float>("Livetime");
        var speedArray = bulletData.Get<Godot.Collections.Array>("Speed");
        bulletSpeedRange = new((float)speedArray[0], (float)speedArray[1]);
    }

    public override float GetBulletSpread() => bulletSpread;
    public override float GetBulletLivetime() => bulletLivetime;
    public override int GetBulletDamage() => Mathf.RoundToInt(bulletDamage * (Player.currentPlayer is null ? 1 : Player.currentPlayer.damageMultiplier));
    public override float GetBulletSpeed() => (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y);

    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }

    protected override void AttackInputProcess(float delta)
    {
        if (allowHold && !isAttacking) Attack();
    }
}
