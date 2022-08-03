using Godot;

public class Rifle : GunBase
{
    protected override void AttackInputProcess()
    {
        if (AnimationPlayer.CurrentAnimation is not "Shoot")
            Attack();
    }
}
