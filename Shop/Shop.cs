using Godot;
using System;

public class Shop : Area2D
{
    [Export] private NodePath shopMenu;
    bool playerInArea;

    [Signal] public delegate void MenuOpened();
    [Signal] public delegate void MenuClosed();
    [Signal] public delegate void PlayerEntered();
    [Signal] public delegate void PlayerExited();

    [TroughtSignal]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        playerInArea = true;
        EmitSignal(nameof(PlayerEntered));
    }

    private void OnAreaExited(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        playerInArea = false;
        EmitSignal(nameof(PlayerExited));
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

        ShopMenu shop = GetNode<ShopMenu>(shopMenu);
        shop.Visible = true;
        shop.OnUpdateRatio();

        EmitSignal(nameof(MenuOpened));
    }
    public void CloseMenu()
    {
        GetTree().Paused = false;

        GetNode<CanvasItem>(shopMenu).Visible = false;

        EmitSignal(nameof(MenuClosed));
    }
}
