using Godot;

public class Rifle : GunBase
{
    protected override void AttackInputProcess(float delta)
    {
        if (!isAttacking) Attack();
    }
}
