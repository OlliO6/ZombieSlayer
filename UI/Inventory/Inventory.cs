using Godot;
using System;

public class Inventory : Control
{
    [Signal] public delegate void OnOpened();
    [Signal] public delegate void OnClosed();
    [Signal] public delegate void OnSelectionChanged(Node to);

    private bool isOpen;

    public ISelectable selection;

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
            ISelectable prevSelected = selection;
            selection = selectable;
            if (prevSelected is not null) prevSelected.Selected = false;
            EmitSignal(nameof(OnSelectionChanged), selection);
            return;
        }

        if (selection == selectable)
        {
            selection = null;
            EmitSignal(nameof(OnSelectionChanged), selection);
        }
    }
}
