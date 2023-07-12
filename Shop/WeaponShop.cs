using Additions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class WeaponShop : Control
{
    private ShopMenu ShopMenu => GetOwner<ShopMenu>();

    public WeaponShopItem selectedItem;
    private Container storerForShopItemsContainer;

    public Container ShopItemsContainer => this.LazyGetNode(ref storerForShopItemsContainer, "%ShopItemsContainer");

    public override void _Ready()
    {
        foreach (var item in ShopItemsContainer.GetChildren().OfType<WeaponShopItem>())
        {
            item.Hide();
            item.UpdateShopItem();
        }
    }

    public void UnlockItem(string name)
    {
        GetItems()
            .FirstOrDefault((ShopItem item) => item.Name == name)
            ?.Show();
    }

    public List<WeaponShopItem> GetItems() => ShopItemsContainer.GetChildren().OfType<WeaponShopItem>().ToList();

    public void SelectItem(WeaponShopItem item)
    {
        selectedItem?.Deselect();
        selectedItem = item;
        ShopMenu.UpdateMenu();
    }

    public int GetPrice() => selectedItem?.GetPrice() ?? -1;

    public Dice MakeDice()
    {
        var scene = selectedItem?.SceneToBuy ?? null;

        Dice dice = ShopMenu.diceScene.Instance<Dice>();
        dice.scenes = new PackedScene[] { scene };

        foreach (var item in GetItems())
            item.Sell();

        SelectItem(null);
        return dice;
    }
}
