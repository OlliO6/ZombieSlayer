using Godot;
using System.Collections.Generic;
using Additions;

public class MagnetArea : Area2D
{
    [Export] public float attractionSpeed = 30, moreAttractionSpeedPerSize = 0.3f, radiusMultiplier = 0.8f;

    #region Collider Reference

    private CollisionShape2D storerForCollider;
    public CollisionShape2D Collider => this.LazyGetNode(ref storerForCollider, "CollisionShape2D");

    #endregion

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            (Collider.Shape as CircleShape2D).Radius = value * radiusMultiplier;
            GD.Print(value * radiusMultiplier);
        }
    }
    private float _size;

    public override void _PhysicsProcess(float delta)
    {
        foreach (Area2D item in GetOverlappingAreas())
        {
            if (item is not PickupBase pickup || !pickup.IsAttractable()) continue;

            Vector2 dir = (GlobalPosition - item.GlobalPosition).Normalized();

            item.GlobalPosition += dir * (attractionSpeed + Size * moreAttractionSpeedPerSize) * delta;
        }
    }
}
