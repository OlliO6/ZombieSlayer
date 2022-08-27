using System;
using System.Linq;
using Additions;
using Godot;

public class DicePickup : InteractionPickupBase
{
    bool isReady = false;
    public Dice Dice
    {
        get => DiceHolder.GetChildCount() > 0 ? DiceHolder.GetChild<Dice>(0) : null;
        set
        {
            Debug.LogU(this, isReady.ToString());
            if (Dice is not null)
            {
                DiceHolder.RemoveChild(Dice);
                Dice.QueueFree();
            }

            DiceHolder.AddChild(value);
            value.Position = Vector2.Zero;
            SetDiceEyeCount();
        }
    }

    #region DiceHolder Reference

    private Node storerForDiceHolder;
    public Node DiceHolder => this.LazyGetNode(ref storerForDiceHolder, "DiceHolder");

    #endregion

    public override void Collect()
    {
        Player.currentPlayer.AddDice(Dice);
    }

    public override bool IsCollectable()
    {
        return Dice is not null;
    }

    private void SetDiceEyeCount()
    {
        Dice.AnimatedSprite.Frame = Dice.scenes.Count((PackedScene scene) => scene is not null) - 1;
    }
}
