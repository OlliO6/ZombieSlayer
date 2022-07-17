using Godot;
using System;

public class Shop : Area2D
{
    [Export] private NodePath shopMenu;
    bool playerInArea;

    [Signal] public delegate void OnMenuOpen();
    [Signal] public delegate void OnMenuClose();
    [Signal] public delegate void OnPlayerEntered();
    [Signal] public delegate void OnPlayerExited();

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        playerInArea = true;
        EmitSignal(nameof(OnPlayerEntered));
    }

    private void OnAreaExited(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        playerInArea = false;
        EmitSignal(nameof(OnPlayerExited));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (playerInArea && @event.IsActionPressed("Interact"))
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        GetTree().Paused = true;

        GetNode<CanvasItem>(shopMenu).Visible = true;

        EmitSignal(nameof(OnMenuOpen));
    }
    public void CloseMenu()
    {
        GetTree().Paused = false;

        GetNode<CanvasItem>(shopMenu).Visible = false;

        EmitSignal(nameof(OnMenuClose));
    }
}
