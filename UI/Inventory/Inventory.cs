using Godot;
using System;

public class Inventory : Control
{
    [Signal] public delegate void OnOpened();
    [Signal] public delegate void OnClosed();

    private bool isOpen;

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
}
