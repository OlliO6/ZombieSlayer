using Additions;
using Godot;

public class Gun : ShootingWeaponBase
{
    public bool allowHold;

    protected override void ApplyData()
    {
        base.ApplyData();

        allowHold = data.Get<bool>("AllowHold");
    }

    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }

    protected override void AttackInputProcess(float delta)
    {
        if (allowHold && !isAttacking) Attack();
    }
}
