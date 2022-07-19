using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShopMenu : Control
{ // TODO Add some rabatt for the buoght stuff that the dice didnt rolled, to do that add some stuff to the dice script
    [Export] private PackedScene diceScene;
    [Export] private NodePath ratioLabel, priceLabel, buyDiceButton, backButton;
    [Export] private NodePath shopItems;

    public override void _Ready()
    {
        OnUpdateRatio();
    }

    [TroughtSignal]
    public void OnUpdateRatio()
    {
        List<ShopItem> items = GetNode(shopItems).GetChildren().OfType<ShopItem>().ToList();

        int ratio = 0;
        int price = 0;
        foreach (var item in items)
        {
            item.UpdateShopItem();
            ratio += item.currentAmount;
            price += item.currentPrice;
        }

        GetNode<Label>(ratioLabel).Text = $"{ratio}/6";
        GetNode<Label>(priceLabel).Text = $"Price: {price}";


        if (ratio >= 6)
        {
            foreach (var item in items)
            {
                item.SetAddButtonEnabled(false);
            }
        }
        else
        {
            foreach (var item in items)
            {
                item.SetAddButtonEnabled(true);
            }
        }

        if (ratio == 0 || Player.currentPlayer is null || Player.currentPlayer.HasDice() || Player.currentPlayer.Coins < price)
        {
            GetNode<Button>(buyDiceButton).Disabled = true;
        }
        else
        {
            GetNode<Button>(buyDiceButton).Disabled = false;
        }
    }

    [TroughtSignal]
    private void OnBuyDicePressed()
    {
        if (Player.currentPlayer is null || Player.currentPlayer.HasDice()) return;

        Dice dice = diceScene.Instance<Dice>();

        List<ShopItem> items = GetNode(shopItems).GetChildren().OfType<ShopItem>().ToList();

        List<PackedScene> scenes = new();

        int price = 0;

        foreach (var item in items)
        {
            for (int i = 0; i < item.currentAmount; i++)
            {
                scenes.Add(item.sceneToBuy);
            }
            price += item.currentPrice;
        }

        if (Player.currentPlayer.Coins < price) return;

        foreach (var item in items) item.Sell();

        AddScenesToDice(dice, scenes);

        Player.currentPlayer.Coins -= price;
        Player.currentPlayer.AddDice(dice);

        OnUpdateRatio();
        GetNode<Button>(backButton).EmitSignal("pressed");
    }

    private void AddScenesToDice(Dice dice, List<PackedScene> scenes)
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            switch (i)
            {
                case 0:
                    dice.scene1 = scenes[0];
                    break;
                case 1:
                    dice.scene2 = scenes[1];
                    break;
                case 2:
                    dice.scene3 = scenes[2];
                    break;
                case 3:
                    dice.scene4 = scenes[3];
                    break;
                case 4:
                    dice.scene5 = scenes[4];
                    break;
                case 5:
                    dice.scene6 = scenes[5];
                    break;
            }
        }
    }
}