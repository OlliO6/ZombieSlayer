using Godot;
using System;
using Additions;

public class Inventory : Control
{
    [Signal] public delegate void OnOpened();
    [Signal] public delegate void OnClosed();
    [Signal] public delegate void OnSelectionChanged(Node to);

    #region DiceContainer Reference

    private DiceContainer storerForDiceContainer;
    public DiceContainer DiceContainer => this.LazyGetNode(ref storerForDiceContainer, _DiceContainer);
    [Export] private NodePath _DiceContainer = "DiceContainer";

    #endregion
    #region CoinLabel Reference

    private Label storerForCoinLabel;
    public Label CoinLabel => this.LazyGetNode(ref storerForCoinLabel, _CoinLabel);
    [Export] private NodePath _CoinLabel = "CoinLabel";

    #endregion

    private bool isOpen;

    private ISelectable selection;

    public ISelectable Selection
    {
        get => selection;
        set
        {
            if (IsInstanceValid(Selection as Node)) Selection.Selected = false;

            selection = value;

            EmitSignal(nameof(OnSelectionChanged), value as Node);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Inventory"))
        {
            if (isOpen) Close();
            else Open();
        }
    }

    private void Open()
    {
        if (GetTree().Paused == true) return;

        isOpen = true;
        GetTree().Paused = true;
        Visible = true;
        Selection = null;

        CoinLabel.Text = Player.currentPlayer is null ? "0" : Player.currentPlayer.Coins.ToString();

        EmitSignal(nameof(OnOpened));
    }

    private void Close()
    {
        isOpen = false;
        GetTree().Paused = false;
        Visible = false;

        EmitSignal(nameof(OnClosed));
    }

    public void SelectionChanged(Node to, bool selected)
    {
        if (to is not ISelectable selectable) return;

        if (selected)
        {
            Selection = selectable;
            return;
        }

        if (Selection == selectable)
        {
            Selection = null;
        }
    }
}
