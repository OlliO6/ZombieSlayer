using Additions;
using Godot;

public class GunBase : WeaponBase
{
    [Export] public PackedScene bulletScene;
    [Export] public Vector2 bulletSpeedRange;
    [Export] public float bulletSpread, bulletLivetime = 3;
    [Export] public int baseBulletDamage;

    #region InstantiatePoint Reference

    private Node2D storerForInstantiatePoint;
    public Node2D InstantiatePoint => this.LazyGetNode(ref storerForInstantiatePoint, _InstantiatePoint);
    [Export] private NodePath _InstantiatePoint = "InstantiatePoint";

    #endregion

    protected Bullet lastBullet;

    public virtual int GetBulletDamageAmount() => Mathf.RoundToInt(baseBulletDamage * (Player.currentPlayer is null ? 1 : Player.currentPlayer.damageMultiplier));

    public override void Attack()
    {
        isAttacking = true;

        lastBullet = bulletScene.Instance<Bullet>();

        lastBullet.speed = (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y);
        lastBullet.DamageAmount = GetBulletDamageAmount();
        lastBullet.maxLivetime = bulletLivetime;

        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: bulletSpread)));

        AnimationPlayer.Stop();
        AnimationPlayer.Play("Shoot");

        EmitSignal(nameof(Attacked));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        Vector2 mousePos = GetGlobalMousePosition();
        LookAt(mousePos);

        if (GlobalPosition.x > mousePos.x)
        {
            Scale = new Vector2(-1, 1);
            Rotate(Mathf.Deg2Rad(180));
        }
        else
        {
            Scale = new Vector2(1, 1);
        }
    }

    protected override void AnimationFinished(string animation)
    {
        if (animation is "Shoot")
        {
            isAttacking = false;
            AttackFinished();
        }
    }
}
