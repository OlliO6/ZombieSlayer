using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class GameState : YSort
{
    [Signal] public delegate void ShopUnlocked();

    [Export] private NodePath shopParent;

    public List<string> unlockedShopItems = new();
    public Shop shop;

    public override void _Ready()
    {
        ExplanationsManager.StartExplanation("MoveAndAttack");
    }

    public void UnlockShop()
    {
        shop = GD.Load<PackedScene>("res://Shop/Shop.tscn").Instance<Shop>();
        GetNode(shopParent).AddChild(shop);

        EmitSignal(nameof(ShopUnlocked));

        foreach (string shopItem in unlockedShopItems)
        {
            shop.UnlockShopItem(shopItem);
        }
    }

    public void UnlockShopItem(string shopItem)
    {
        unlockedShopItems.Add(shopItem);

        if (IsInstanceValid(shop))
        {
            shop.UnlockShopItem(shopItem);
        }
    }
}
