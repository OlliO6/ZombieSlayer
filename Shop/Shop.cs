using System;
using Additions;
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

    private void SetExitButtonDisabled(bool disabled) => shop.GetNode<GameButton>("%ExitButton").Disabled = disabled;

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
        PauseMode = PauseModeEnum.Process;
        GetTree().Paused = true;

        shop.Visible = true;
        shop.UpdateMenu();

        EmitSignal(nameof(MenuOpened));

        if (dialogPlayer.IsInDialog)
        {
            ToSignal(dialogPlayer, nameof(DialogPlayer.DialogFinished))
                .OnCompleted(shop.GrabUIFocus);
            return;
        }
        shop.GrabUIFocus();
    }

    public void CloseMenu()
    {
        if (dialogPlayer.IsInDialog)
            return;

        PauseMode = PauseModeEnum.Inherit;
        GetTree().Paused = false;
        shop.Visible = false;

        EmitSignal(nameof(MenuClosed));
    }

    public void Interact()
    {
        PauseMode = PauseModeEnum.Process;
        GetTree().Paused = true;

        if (!hadFirstTalk)
        {
            SetExitButtonDisabled(true);
            dialogPlayer.Play("FirstMeeting");
            ToSignal(dialogPlayer, "DialogFinished").OnCompleted(() =>
            {
                SetExitButtonDisabled(false);
                hadFirstTalk = true;
            });
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

    [TroughtEditor]
    private void OnShopRobbed()
    {
        GetTree().Paused = true;
        SetExitButtonDisabled(true);
        dialogPlayer.Play("Robbery");
        ToSignal(dialogPlayer, "DialogFinished").OnCompleted(() =>
        {
            SetExitButtonDisabled(false);
            GetTree().Paused = false;
        });
    }

    public void StartFigth()
    {
        Debug.LogU(this, "Start Fight");
        Transitions.StartTransition(Transitions.TransitionBlocks, 0.5f, () =>
        {
            // SceneManager.ChangeScence(GD.Load<PackedScene>("res://RobFights/FirstFight/FirstRobFight.tscn"));
            Transitions.EndTransition(Transitions.TransitionBlocks, unpause: false);
        });
    }
}
