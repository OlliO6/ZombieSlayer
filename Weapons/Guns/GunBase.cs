using Additions;
using Godot;
using System;

public class GunBase : WeaponBase
{
    [Export] private PackedScene bulletScene;
    [Export] private Vector2 bulletSpeedRange;
    [Export] private float bulletSpread;


    #region InstantiatePoint Reference

    private Node2D storerForInstantiatePoint;
    public Node2D InstantiatePoint => this.LazyGetNode(ref storerForInstantiatePoint, _InstantiatePoint);
    [Export] private NodePath _InstantiatePoint = "InstantiatePoint";

    #endregion

    protected Bullet lastBullet;

    public override void Attack()
    {
        lastBullet = bulletScene.Instance<Bullet>();

        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;

        lastBullet.Rotate(Mathf.Deg2Rad((float)GD.RandRange(-bulletSpread, bulletSpread)));

        lastBullet.speed = (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y);
    }
}
