using Godot;
using System.Linq;
using Additions;

public class Dice : KinematicBody2D
{
    [Export] public PackedScene[] scenes;
    [Export] public bool broken;

    [Export] private PackedScene pickupScene;

    [Export] private Vector3 minLaunchVelocity, maxLaunchVelocity;
    [Export] private float launchGravity = 9.8f;
    [Export(PropertyHint.Range, "0,1")] private float bounciness = 0.8f, speedRemainWhenBounce = 0.8f;

    private bool isRolling;
    private Vector3 launchVelocity;
    private float rollSpeed;

    #region AnimatedSprite Reference

    private AnimatedSprite storerForSprite;
    public AnimatedSprite AnimatedSprite => this.LazyGetNode(ref storerForSprite, "AnimatedSprite");

    #endregion

    public override void _Ready()
    {
        AnimatedSprite.Frame = Mathf.RoundToInt((float)GD.RandRange(0, AnimatedSprite.Frames.GetFrameCount(AnimatedSprite.Animation) - 1));
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

            launchVelocity.z *= -bounciness;

            rollSpeed *= speedRemainWhenBounce;

            if (rollSpeed <= 0.05f)
            {
                Finish();
            }
        }
    }

    private void Finish()
    {
        isRolling = false;
        broken = true;
        AnimatedSprite.SpeedScale = 0;

        SpawnRandomScene();

        if (scenes.All((PackedScene scene) => scene is null))
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("BreakComplete");
            return;
        }

        SpawnBrokenDicePickup();

        GetNode<AnimationPlayer>("AnimationPlayer").Play("Break");
    }

    private void SpawnBrokenDicePickup()
    {
        DicePickup pickup = pickupScene.Instance<DicePickup>();

        GetParent().AddChild(pickup);
        pickup.GlobalPosition = GlobalPosition + Vector2.Up;

        GetParent().RemoveChild(this);
        pickup.Dice = this;
    }

    private void SpawnRandomScene()
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();

        Node2D instance = RandomInstance(rng);

        if (instance is null)
        {
            GetNode<AudioStreamPlayer>("LoosePlayer").Play();
            return;
        }

        GetNode<AudioStreamPlayer>("WinPlayer").Play();

        GetParent().AddChild(instance);
        instance.GlobalPosition = GlobalPosition;

        Node2D RandomInstance(RandomNumberGenerator rng)
        {
            if (scenes is null || scenes.Length is 0) return null;

            int index = rng.RandiRange(0, scenes.Length - 1);

            PackedScene scene = scenes[index];
            RemoveAndFitFromArray(index);

            return scene is null ? null : scene.Instance<Node2D>();
        }

        void RemoveAndFitFromArray(int index)
        {
            scenes[index] = null;

            for (int i = index; i < scenes.Length; i++)
            {
                scenes[i] = i + 1 < scenes.Length ? scenes[i + 1] : null;
            }
        }
    }
}
