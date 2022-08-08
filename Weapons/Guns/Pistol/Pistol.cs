using Godot;

public class Pistol : GunBase
{
    protected override void AttackInputStarted()
    {
        if (!isAttacking) Attack();
    }
}
