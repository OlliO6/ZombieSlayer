using System;
using Godot;

public class Shop : Area2D
{
    [Signal] public delegate void MenuOpened();
    [Signal] public delegate void MenuClosed();
    [Signal] public delegate void PlayerEntered();
    [Signal] public delegate void PlayerExited();

    [Export] private NodePath shopMenu;
    private bool playerInArea;

    private ShopMenu shop;

    public override void _Ready()
    {
        shop = GetNode<ShopMenu>(shopMenu);

        ExplanationsManager.ConnectExplanationToSignal("ShopMenu", this, nameof(MenuOpened));
    }

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

    public void UnlockShopItem(string name) => shop.UnlockItem(name);

    public void OpenMenu()
    {
        GetTree().Paused = true;

        shop.Visible = true;
        shop.OnUpdateRatio();

        EmitSignal(nameof(MenuOpened));
    }
    public void CloseMenu()
    {
        GetTree().Paused = false;
        shop.Visible = false;

        EmitSignal(nameof(MenuClosed));
    }
}
