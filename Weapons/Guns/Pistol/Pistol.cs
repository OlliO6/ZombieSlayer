using Godot;

public class Pistol : GunBase
{
    protected override void AttackInputStarted()
    {
        if (AnimationPlayer.CurrentAnimation is not "Shoot")
            Attack();
    }
}
