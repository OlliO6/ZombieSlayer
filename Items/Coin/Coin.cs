using Godot;
using System;
using Additions;

public class Coin : Area2D
{
    [Export] public int amount;
    [Export] private Vector3 minLaunchVelocity, maxLaunchVelocity;
    [Export] private float launchGravity = 9.8f;

    [Signal] public delegate void OnCollected();

    #region Sprite Reference

    private Sprite storerForSprite;
    public Sprite Sprite => this.LazyGetNode(ref storerForSprite, "Sprite");

    #endregion

    private bool isLaunching;
    private Vector3 launchVelocity;

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        Player.currentPlayer.Coins += amount;

        EmitSignal(nameof(OnCollected));
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
}
