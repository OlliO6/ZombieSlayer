using Godot;

public class Rifle : GunBase
{
    protected override void AttackInputProcess()
    {
        if (!isAttacking) Attack();
    }
}
