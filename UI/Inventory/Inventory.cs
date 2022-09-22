using System;
using Additions;
using Godot;

public class Inventory : Control
{
    [Signal] public delegate void Opened();
    [Signal] public delegate void Closed();
    [Signal] public delegate void SelectionChanged(Node to);

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
            if (IsInstanceValid(Selection as Node)) Selection.IsSelected = false;

            selection = value;

            EmitSignal(nameof(SelectionChanged), value as Node);
        }
    }

    public override void _EnterTree()
    {
        InputManager.InventoryPressed += OnInventoryPressed;
        InputManager.UICancelPressed += OnUICancelPressed;
    }
    public override void _ExitTree()
    {
        InputManager.InventoryPressed -= OnInventoryPressed;
        InputManager.UICancelPressed -= OnUICancelPressed;
    }

    private void OnInventoryPressed()
    {
        if (isOpen) Close();
        else Open();
    }
    private void OnUICancelPressed()
    {
        if (isOpen) Close();
    }

    private void Open()
    {
        if (GetTree().Paused) return;

        isOpen = true;
        GetTree().Paused = true;
        Visible = true;
        Selection = null;

        CoinLabel.Text = Player.currentPlayer is null ? "0" : Player.currentPlayer.Coins.ToString();

        EmitSignal(nameof(Opened));
    }

    private void Close()
    {
        isOpen = false;
        GetTree().Paused = false;
        Visible = false;

        EmitSignal(nameof(Closed));
    }

    public void FieldSelected(Node field, bool selected)
    {
        if (field is not ISelectable selectable) return;

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
