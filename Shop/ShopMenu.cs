using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class ShopMenu : Control
{
    [Signal] public delegate void BoughtDice();
    [Signal] public delegate void Robbed();

    [Export] public PackedScene diceScene;
    [Export] public int allowRobberyPrice = 1000;

    private UpgradeShop storerForUpgradeShop;
    private WeaponShop storerForWeaponShop;
    private Label storerForPriceLabel;
    private Button storerForBuyDiceButton;
    private Button storerForRobButton;
    private Button storerForSwitchShopButton;

    public UpgradeShop UpgradeShop => this.LazyGetNode(ref storerForUpgradeShop, "%UpgradeShop");
    public WeaponShop WeaponShop => this.LazyGetNode(ref storerForWeaponShop, "%WeaponShop");
    public Button BuyDiceButton => this.LazyGetNode(ref storerForBuyDiceButton, "%BuyDiceButton");
    public Button RobButton => this.LazyGetNode(ref storerForRobButton, "%RobButton");
    public Button SwitchShopButton => this.LazyGetNode(ref storerForSwitchShopButton, "%SwitchShopButton");
    public Label PriceLabel => this.LazyGetNode(ref storerForPriceLabel, "%PriceLabel");

    public override void _Ready()
    {
        UpdateMenu();
    }

    public void UpdateMenu()
    {
        var price = GetPrice();

        PriceLabel.Text = $"Price: {Mathf.Max(0, price).ToString()}";
        BuyDiceButton.Disabled = (price < 0 || Player.currentPlayer is null || Player.currentPlayer.Coins < price) ? true : false;
        RobButton.Visible = price >= allowRobberyPrice;
        SwitchShopButton.Text = UpgradeShop.Visible ? "Weapons" : "Upgrades";
    }

    public int GetPrice()
    {
        if (UpgradeShop.Visible)
            return UpgradeShop.GetPrice();

        if (WeaponShop.Visible)
            return WeaponShop.GetPrice();

        return 0;
    }

    public void UnlockItem(string name)
    {
        UpgradeShop.UnlockItem(name);
        WeaponShop.UnlockItem(name);
    }

    public void GrabUIFocus()
    {
        if (UpgradeShop.Visible)
        {
            foreach (var child in UpgradeShop.ShopItemsContainer.GetChildren<UpgradeShopItem>())
            {
                if (child.IsVisibleInTree())
                {
                    child.GetAddButton().GrabFocus();
                    return;
                }
            }
            return;
        }
        foreach (var child in WeaponShop.ShopItemsContainer.GetChildren<WeaponShopItem>())
        {
            if (child.IsVisibleInTree())
            {
                child.GetNode<GameButton>("Button").GrabFocus();
                return;
            }
        }
        GetNode<GameButton>("ExitButton").GrabFocus();
    }

    public void ReleaseUIFocus()
    {
        GetFocusOwner().ReleaseFocus();
    }

    [TroughtEditor]
    private void OnBackPressed() => (Owner as Shop)?.CloseMenu();

    [TroughtEditor]
    private void OnBuyDicePressed()
    {
        if (Player.currentPlayer is null) return;

        int price = GetPrice();

        if (Player.currentPlayer.Coins < price)
            return;

        Dice dice = UpgradeShop.Visible ?
            UpgradeShop.MakeDice() :
            WeaponShop.MakeDice();

        Player.currentPlayer.Coins -= price;
        Player.currentPlayer.AddDice(dice);

        EmitSignal(nameof(BoughtDice));
        UpdateMenu();
        Debug.LogU(this, "Selled Dice");
    }

    [TroughtEditor]
    private void MakeRobbery()
    {
        if (Player.currentPlayer is null) return;

        Dice dice = UpgradeShop.Visible ?
            UpgradeShop.MakeDice() :
            WeaponShop.MakeDice();

        Player.currentPlayer.AddDice(dice);

        EmitSignal(nameof(Robbed));
        UpdateMenu();
        Debug.LogU(this, "Rob got robbed.");
    }

    [TroughtEditor]
    private void OnSwitchShopPressed()
    {
        UpgradeShop.Visible = !UpgradeShop.Visible;
        WeaponShop.Visible = !WeaponShop.Visible;
        UpdateMenu();
    }
}
