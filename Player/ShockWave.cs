using Godot;
using System;
using Additions;
using Shaking;
using System.Collections.Generic;
using Enemies;

public class ShockWave : Sprite
{
    [Export] public float baseRadius;
    [Export] public int damageAmount;
    [Export] public float knockbackSpeed;
    [Export] public float stunnTime;

    public bool isShocking;

    private List<IDamageable> _damaged;

    public float Radius => baseRadius * CollisionShape.Scale.x;

    private CollisionShape2D _collisionShape;
    public CollisionShape2D CollisionShape => this.LazyGetNode(ref _collisionShape, "Area2D/CollisionShape2D");

    private Area2D _area;
    public Area2D Area => this.LazyGetNode(ref _area, "Area2D");

    private AnimationPlayer _anim;
    public AnimationPlayer Anim => this.LazyGetNode(ref _anim, "AnimationPlayer");

    public bool AllowDamageTo(IDamageable to) => true;


    public async void InduceShockWave()
    {
        if (isShocking)
            return;

        isShocking = true;
        _damaged = new();
        Anim.Play("shock");
        await ToSignal(Anim, "animation_finished");
        _damaged = null;
        isShocking = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!isShocking)
            return;

        foreach (Node node in Area.GetOverlappingBodies())
        {
            if (node is IDamageable damageable && !_damaged.Contains(damageable))
            {
                _damaged.Add(damageable);
                damageable.GetDamage(damageAmount);
                (damageable as IStunnable)?.Stunn(stunnTime);
            }

            if (node is KinematicBody2D kinematicBody)
            {
                var dir = GlobalPosition.DirectionTo(kinematicBody.GlobalPosition);
                var targetPos = GlobalPosition + dir * Radius;
                kinematicBody.MoveAndSlide((targetPos - kinematicBody.GlobalPosition) * knockbackSpeed);
            }
        }
    }
}
