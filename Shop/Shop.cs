using System;
using Diadot;
using Godot;

public class Shop : Area2D
{
    [Signal] public delegate void MenuOpened();
    [Signal] public delegate void MenuClosed();
    [Signal] public delegate void PlayerEntered();
    [Signal] public delegate void PlayerExited();

    private bool playerInArea, hadFirstTalk;
    private ShopMenu shop;
    private DialogPlayer dialogPlayer;

    public override void _Ready()
    {
        shop = GetNode<ShopMenu>("%ShopMenu");
        dialogPlayer = GetNode<DialogPlayer>("DialogPlayer");
    }

    public override void _EnterTree()
    {
        InputManager.UICancelPressed += OnUICancelPressed;
    }
    public override void _ExitTree()
    {
        InputManager.UICancelPressed -= OnUICancelPressed;
    }

    private void OnUICancelPressed()
    {
        if (shop.Visible)
            CloseMenu();
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
            TalkToPlayer();
        }
    }

    private async void TalkToPlayer()
    {
        if (!hadFirstTalk)
        {
            hadFirstTalk = true;
            GetTree().Paused = true;
            dialogPlayer.Play("FirstMeeting");
            await ToSignal(dialogPlayer, nameof(DialogPlayer.DialogFinished));
        }
        OpenMenu();
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
