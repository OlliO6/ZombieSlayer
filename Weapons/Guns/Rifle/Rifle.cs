using Godot;
using System;

public class Rifle : GunBase
{
    protected override void AttackInputProcess()
    {
        Attack();
    }
}
