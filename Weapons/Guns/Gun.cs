using Godot;

public class Gun : ShootingWeaponBase
{
    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }
}
