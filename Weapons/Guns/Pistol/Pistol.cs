using Godot;
using System;

public class Pistol : GunBase
{
    protected override void AttackInputStarted()
    {
        Attack();
    }
}
