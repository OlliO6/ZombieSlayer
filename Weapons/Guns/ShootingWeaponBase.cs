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
        bulletScene = GD.Load<PackedScene>(bulletData.Get<string>("Path"));
    }

    public virtual int GetBulletDamage() => 0;
    public virtual float GetBulletSpread() => 0;
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
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: GetBulletSpread())));

        AnimationPlayer.Stop();
        AnimationPlayer.Play("Shoot");

        EmitSignal(nameof(AttackStarted));
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

    protected override void OnAnimationFinished(string animation)
    {
        if (animation is "Shoot")
        {
            isAttacking = false;
            OnAttackFinished();
        }
    }
}
