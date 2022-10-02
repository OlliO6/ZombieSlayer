using System;
using Diadot;
using Godot;

public class Shop : Area2D, IInteractable
{
    [Signal] public delegate void MenuOpened();
    [Signal] public delegate void MenuClosed();
    [Signal] public delegate void PlayerEntered();
    [Signal] public delegate void PlayerExited();

    private bool hadFirstTalk;
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

    [TroughtEditor]
    private void OnAreaEntered(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        Player.currentPlayer.AddInteractable(this);
    }

    private void OnAreaExited(Area2D area)
    {
        if (Player.currentPlayer is null) return;

        Player.currentPlayer.RemoveInteractable(this);
    }

    public void UnlockShopItem(string name) => shop.UnlockItem(name);

    public void OpenMenu()
    {
        GetTree().Paused = true;

        shop.Visible = true;
        shop.OnDiceUpdate();

        EmitSignal(nameof(MenuOpened));
    }
    public void CloseMenu()
    {
        GetTree().Paused = false;
        shop.Visible = false;

        EmitSignal(nameof(MenuClosed));
    }

    public void Interact()
    {
        if (!hadFirstTalk)
        {
            InputManager.ProcessInput = false;
            hadFirstTalk = true;
            GetTree().Paused = true;
            dialogPlayer.Play("FirstMeeting");
            ToSignal(dialogPlayer, "DialogFinished").OnCompleted(
                    () => { InputManager.ProcessInput = true; });
            return;
        }
        OpenMenu();
    }

    public void Select()
    {
        EmitSignal(nameof(PlayerEntered));
    }

    public void Deselect()
    {
        EmitSignal(nameof(PlayerExited));
    }
}
