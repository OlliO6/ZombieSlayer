using System;
using Additions;
using Godot;

[Additions.Debugging.DefaultColor("gold")]
public class Coin : PickupBase
{
    [Export] public int amount;
    [Export] private Vector3 minLaunchVelocity, maxLaunchVelocity;
    [Export] private float launchGravity = 9.8f;

    #region Sprite Reference

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    #endregion

    private bool isLaunching;
    private Vector3 launchVelocity;

    public override void Collect()
    {
        Player.currentPlayer.Coins += amount;
    }

    public void Launch()
    {
        isLaunching = true;

        RandomNumberGenerator rng = new();
        rng.Randomize();

        launchVelocity = new Vector3(rng.RandfRange(minLaunchVelocity.x, maxLaunchVelocity.y),
            rng.RandfRange(minLaunchVelocity.y, maxLaunchVelocity.y),
            rng.RandfRange(minLaunchVelocity.z, maxLaunchVelocity.z));
    }

    public override void _Process(float delta)
    {
        if (!isLaunching) return;

        launchVelocity.z += launchGravity * delta;

        Position += new Vector2(launchVelocity.x, launchVelocity.y) * delta;

        Sprite.Position += new Vector2(0, launchVelocity.z) * delta;

        if (Sprite.Position.y > 0)
        {
            Sprite.Position = Vector2.Zero;
            isLaunching = false;
        }
    }

    public override bool IsAttractable() => !isLaunching || launchVelocity.z > -5;
}
