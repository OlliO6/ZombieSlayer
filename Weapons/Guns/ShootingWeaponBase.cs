using Additions;
using Godot;

public abstract class ShootingWeaponBase : WeaponBase
{
    protected Godot.Collections.Dictionary bulletData;

    public PackedScene bulletScene;
    public Vector2 bulletSpeedRange;
    public float bulletSpread, bulletLivetime = 3;
    public int bulletDamage;

    public Node2D InstantiatePoint => this.LazyGetNode(ref storerForInstantiatePoint, _InstantiatePoint);
    protected Bullet lastBullet;

    [Export] private NodePath _InstantiatePoint = "InstantiatePoint";
    private Node2D storerForInstantiatePoint;

    protected override void ApplyData()
    {
        base.ApplyData();

        bulletData = data.Get<Godot.Collections.Dictionary>("Bullet");
        bulletScene = GD.Load<PackedScene>(bulletData.Get<string>("Path"));
        bulletSpread = bulletData.Get<float>("Spread");
        bulletLivetime = bulletData.Get<float>("Livetime");
        bulletDamage = (int)bulletData.Get<float>("Damage");
        var speedArray = bulletData.Get<Godot.Collections.Array>("Speed");
        bulletSpeedRange = new((float)speedArray[0], (float)speedArray[1]);
    }

    public virtual int GetBulletDamage() => Mathf.RoundToInt(bulletDamage * (Player.currentPlayer is null ? 1 : Player.currentPlayer.damageMultiplier));

    public override void Attack()
    {
        isAttacking = true;

        lastBullet = bulletScene.Instance<Bullet>();

        lastBullet.speed = (float)GD.RandRange(bulletSpeedRange.x, bulletSpeedRange.y);
        lastBullet.damage = GetBulletDamage();
        lastBullet.maxLivetime = bulletLivetime;

        GetTree().CurrentScene.AddChild(lastBullet);
        lastBullet.GlobalTransform = InstantiatePoint.GlobalTransform;
        lastBullet.Rotate(Mathf.Deg2Rad(Random.NormallyDistributedFloat(deviation: bulletSpread)));

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
