using Additions;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradeShop : HBoxContainer
{
    private ShopMenu ShopMenu => GetOwner<ShopMenu>();

    private Container storerForShopItemsContainer;
    private DiceScenesContainer storerForDiceScenesContainer;
    private List<UpgradeShopItem> selectedItems = new();

    public Container ShopItemsContainer => this.LazyGetNode(ref storerForShopItemsContainer, "%ShopItemsContainer");
    public DiceScenesContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, "%DiceScenesContainer");

    public override void _Ready()
    {
        foreach (var item in ShopItemsContainer.GetChildren().OfType<UpgradeShopItem>())
        {
            item.Hide();
            item.UpdateShopItem();
        }
        DiceUpdate();

        DiceScenesContainer.Connect(nameof(DiceScenesContainer.Interacted), this, nameof(OnSceneFieldInteracted));
        DiceScenesContainer.Connect(nameof(DiceScenesContainer.LostFocus), ShopMenu, nameof(ShopMenu.GrabUIFocus));
    }

    private void OnSceneFieldInteracted(int index)
    {
        if (selectedItems.Count >= index + 1)
        {
            var removingItem = selectedItems[index];

            removingItem.currentAmount--;
            removingItem.count--;
            removingItem.UpdateShopItem();

            selectedItems.RemoveAt(index);
            DiceUpdate();
        }
    }

    public void DiceUpdate()
    {
        var items = GetItems();
        var scenes = GetScenesToBuy();

        foreach (var item in items)
            item.SetAddButtonEnabled(scenes.Count < 6);

        // Make Scene count 6 
        while (scenes.Count < 6)
            scenes.Add(null);

        DiceScenesContainer.Scenes = scenes;

        ShopMenu.UpdateMenu();
    }

    public void OnItemRemoved(UpgradeShopItem item)
    {
        selectedItems.Remove(item);
        DiceUpdate();
    }

    public void OnItemAdded(UpgradeShopItem item)
    {
        selectedItems.Add(item);
        DiceUpdate();
    }

    public List<UpgradeShopItem> GetItems() => ShopItemsContainer.GetChildren().OfType<UpgradeShopItem>().ToList();

    public List<PackedScene> GetScenesToBuy() => selectedItems.ConvertAll((UpgradeShopItem item) => item.SceneToBuy);

    public Dice MakeDice()
    {
        const int sideCount = 6;
        var scenes = GetScenesToBuy();

        Dice dice = ShopMenu.diceScene.Instance<Dice>();
        dice.scenes = new PackedScene[sideCount];

        for (int i = 0; i < (scenes.Count > sideCount ? sideCount : scenes.Count); i++)
            dice.scenes[i] = scenes[i];

        foreach (var item in GetItems())
            item.Sell();

        selectedItems = new();
        DiceUpdate();

        return dice;
    }

    public int GetPrice()
    {
        int result = 0;

        if (selectedItems.Count == 0)
            return -1;

        foreach (UpgradeShopItem item in GetItems())
        {
            for (int count = item.count - 1; count >= item.count - item.currentAmount; count--)
            {
                result += item.GetPrice(count);
            }
        }

        return result;
    }

    public void UnlockItem(string name)
    {
        GetItems()
            .FirstOrDefault((ShopItem item) => item.Name == name)
            ?.Show();
    }
}
