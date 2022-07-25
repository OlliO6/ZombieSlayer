using Godot;
using System.Collections.Generic;
using Additions;

public class DiceInventory : Node2D
{
    #region WorkingDices Reference

    private Node2D storerForWorkingDices;
    public Node2D WorkingDices => this.LazyGetNode(ref storerForWorkingDices, "WorkingDices");

    #endregion

    #region BrokenDices Reference

    private Node2D storerForBrokenDices;
    public Node2D BrokenDices => this.LazyGetNode(ref storerForBrokenDices, "BrokenDices");

    #endregion

    public void AddDice(Dice dice)
    {
        dice.GetParent()?.RemoveChild(dice);

        (dice.broken ? BrokenDices : WorkingDices).AddChild(dice);

        dice.Visible = false;
    }
    public IEnumerable<Dice> GetWorkingDices() => WorkingDices.GetChildren<Dice>();
    public IEnumerable<Dice> GetBrokenDices() => BrokenDices.GetChildren<Dice>();
}
