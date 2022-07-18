using Godot;
using System;
using Additions;

public class Dice : KinematicBody2D
{
    [Export] public PackedScene scene1, scene2, scene3, scene4, scene5, scene6;

    [Export] private Vector3 minLaunchVelocity, maxLaunchVelocity;
    [Export] private float launchGravity = 9.8f;

    private bool isRolling, done;
    private Vector3 launchVelocity;
    private float rollSpeed;

    #region AnimatedSprite Reference

    private AnimatedSprite storerForSprite;
    public AnimatedSprite AnimatedSprite => this.LazyGetNode(ref storerForSprite, "AnimatedSprite");

    #endregion

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!isRolling && !done && @event.IsActionPressed("Interact"))
        {
            Launch();
        }
    }

    public void Launch()
    {
        isRolling = true;

        RandomNumberGenerator rng = new();
        rng.Randomize();

        Node newParent = GetTree().CurrentScene;
        Vector2 oldPos = GlobalPosition;
        GetParent().RemoveChild(this);
        newParent.AddChild(this);
        GlobalPosition = oldPos;

        rollSpeed = 1;

        launchVelocity = new Vector3(rng.RandfRange(minLaunchVelocity.x, maxLaunchVelocity.y),
            rng.RandfRange(minLaunchVelocity.y, maxLaunchVelocity.y),
            rng.RandfRange(minLaunchVelocity.z, maxLaunchVelocity.z));
    }

    public override void _Process(float delta)
    {
        if (!isRolling) return;

        launchVelocity.z += launchGravity * delta;

        MoveAndSlide(new Vector2(launchVelocity.x, launchVelocity.y) * rollSpeed);

        AnimatedSprite.Position += new Vector2(0, launchVelocity.z) * delta;
        AnimatedSprite.SpeedScale = rollSpeed;

        if (AnimatedSprite.Position.y > 0)
        {
            AnimatedSprite.Position = Vector2.Zero;
            GetNode<AudioStreamPlayer>("RollPlayer").Play();

            launchVelocity.z *= -0.6f;

            rollSpeed -= 0.2f;

            if (rollSpeed <= 0)
            {
                isRolling = false;
                done = true;
                AnimatedSprite.SpeedScale = 0;

                SpawnRandomScene();
            }
        }
    }

    private void SpawnRandomScene()
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();

        int scene = rng.RandiRange(1, 6);

        Node2D instance = null;

        switch (scene)
        {
            case 1:
                instance = scene1?.Instance<Node2D>();
                break;
            case 2:
                instance = scene2?.Instance<Node2D>();
                break;
            case 3:
                instance = scene3?.Instance<Node2D>();
                break;
            case 4:
                instance = scene4?.Instance<Node2D>();
                break;
            case 5:
                instance = scene5?.Instance<Node2D>();
                break;
            case 6:
                instance = scene6?.Instance<Node2D>();
                break;
        }

        if (instance is null)
        {
            GetNode<AudioStreamPlayer>("LoosePlayer").Play();
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Finish");
            return;
        }

        GetNode<AudioStreamPlayer>("WinPlayer").Play();

        GetParent().AddChild(instance);
        instance.GlobalPosition = GlobalPosition;

        GetNode<AnimationPlayer>("AnimationPlayer").Play("Finish");
    }
}
