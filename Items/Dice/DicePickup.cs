using Godot;
using System;
using Additions;

public class DicePickup : InteractionPickupBase
{
    public Dice Dice
    {
        get => DiceHolder.GetChildCount() > 0 ? DiceHolder.GetChild<Dice>(0) : null;
        set
        {
            if (Dice is not null)
            {
                DiceHolder.RemoveChild(Dice);
                Dice.QueueFree();
            }

            DiceHolder.AddChild(value);
            value.Position = Vector2.Zero;
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
    public override void _Ready()
    {

    }
}
