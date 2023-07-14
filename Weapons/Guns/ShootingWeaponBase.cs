using Additions;
using Godot;

public abstract class ShootingWeaponBase : WeaponBase
{
    protected Godot.Collections.Dictionary bulletData;

    public PackedScene bulletScene;

    public Node2D InstantiatePoint => this.LazyGetNode(ref storerForInstantiatePoint, _InstantiatePoint);
    protected Bullet lastBullet;

    [Export] private NodePath _InstantiatePoint = "InstantiatePoint";
    private Node2D storerForInstantiatePoint;

    protected override void ApplyData()
    {
        base.ApplyData();

        bulletData = data.Get<Godot.Collections.Dictionary>("Bullet");
        bulletScene = bulletData.Get<PackedScene>("Scene");
    }

    public virtual float GetSpread() => 0;
    public virtual int GetBulletDamage() => 0;
    public virtual float GetBulletSpeed() => 0;
    public virtual float GetBulletLivetime() => 0;

    public override void Attack()
    {
        isAttacking = true;

        lastBullet = bulletScene.Instance<Bullet>();
        lastBullet.damage = GetBulletDamage();
        lastBullet.maxLivetime = GetBulletLivetime();
        lastBullet.speed = GetBulletSpeed();

        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: GetSpread())));

        AnimationPlayer.Stop();
        AnimationPlayer.Play("Shoot");

        EmitSignal(nameof(AttackStarted));
    }

    protected override void OnAnimationFinished(string animation)
    {
        if (animation is "Shoot")
        {
            isAttacking = false;
            OnAttackFinished();
        }
    }
}
