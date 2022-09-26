using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class ShopMenu : Control
{
    [Export] private PackedScene diceScene;
    [Export] private NodePath ratioLabel, priceLabel, buyDiceButton, backButton;
    [Export] private NodePath shopItems;

    public override void _Ready()
    {
        foreach (ShopItem item in GetNode(shopItems).GetChildren().OfType<ShopItem>())
        {
            item.Hide();
        }
        OnUpdateRatio();
    }

    [TroughtEditor]
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

        if (ratio == 0 || Player.currentPlayer is null || Player.currentPlayer.Coins < price)
        {
            GetNode<Button>(buyDiceButton).Disabled = true;
        }
        else
        {
            GetNode<Button>(buyDiceButton).Disabled = false;
        }
    }

    public void UnlockItem(string name)
    {
        GetNode(shopItems).GetChildren().OfType<ShopItem>().First((ShopItem item) => item.Name == name)
                .Show();
    }

    [TroughtEditor]
    private void OnBuyDicePressed()
    {
        if (Player.currentPlayer is null) return;

        Dice dice = diceScene.Instance<Dice>();

        List<ShopItem> items = GetNode(shopItems).GetChildren().OfType<ShopItem>().ToList();

        List<PackedScene> scenes = new();

        int price = 0;

        foreach (var item in items)
        {
            for (int i = 0; i < item.currentAmount; i++)
            {
                scenes.Add(item.SceneToBuy);
            }
            price += item.currentPrice;
        }

        if (Player.currentPlayer.Coins < price) return;

        foreach (var item in items) item.Sell();

        AddScenesToDice(dice, scenes);

        Player.currentPlayer.Coins -= price;
        Player.currentPlayer.AddDice(dice);

        OnUpdateRatio();

        Debug.LogU(this, "Selled Dice");
    }

    private void AddScenesToDice(Dice dice, List<PackedScene> scenes)
    {
        const int sideCount = 6;

        dice.scenes = new PackedScene[sideCount];

        for (int i = 0; i < (scenes.Count > sideCount ? sideCount : scenes.Count); i++)
        {
            dice.scenes[i] = scenes[i];
        }
    }
}
