using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class ShopMenu : Control
{
    [Signal] public delegate void BoughtDice();

    [Export] private PackedScene diceScene;

    public List<ShopItem> itemsToBuy = new();

    private Label storerForPriceLabel;
    private Container storerForShopItemsContainer;
    private Button storerForBuyDiceButton;
    private DiceScenesContainer storerForDiceScenesContainer;

    public Button BuyDiceButton => this.LazyGetNode(ref storerForBuyDiceButton, "%BuyDiceButton");
    public Label PriceLabel => this.LazyGetNode(ref storerForPriceLabel, "%PriceLabel");
    public Container ShopItemsContainer => this.LazyGetNode(ref storerForShopItemsContainer, "%ShopItemsContainer");
    public DiceScenesContainer DiceScenesContainer => this.LazyGetNode(ref storerForDiceScenesContainer, "%DiceScenesContainer");

    public override void _Ready()
    {
        foreach (ShopItem item in ShopItemsContainer.GetChildren().OfType<ShopItem>())
        {
            item.Hide();
            item.UpdateShopItem();
        }
        DiceUpdate();

        DiceScenesContainer.Connect(nameof(DiceScenesContainer.Interacted), this, nameof(OnSceneFieldInteracted));
    }

    private void OnSceneFieldInteracted(int index)
    {
        if (itemsToBuy.Count >= index + 1)
        {
            var removingItem = itemsToBuy[index];

            removingItem.currentAmount--;
            removingItem.count--;
            removingItem.UpdateShopItem();

            itemsToBuy.RemoveAt(index);
            DiceUpdate();
        }
    }

    public void DiceUpdate()
    {
        var items = GetItems();
        var scenes = GetScenesToBuy();
        int price = GetPrice();

        foreach (ShopItem item in items)
            item.SetAddButtonEnabled(scenes.Count < 6);

        BuyDiceButton.Disabled = (scenes.Count is 0 || Player.currentPlayer is null || Player.currentPlayer.Coins < price) ? true : false;

        // Make Scene count 6 
        while (scenes.Count < 6)
            scenes.Add(null);

        DiceScenesContainer.Scenes = scenes;
        PriceLabel.Text = $"Price: {price.ToString()}";
    }

    public void OnItemRemoved(ShopItem item)
    {
        itemsToBuy.Remove(item);
        DiceUpdate();
    }

    public void OnItemAdded(ShopItem item)
    {
        itemsToBuy.Add(item);
        DiceUpdate();
    }

    public List<ShopItem> GetItems() => ShopItemsContainer.GetChildren().OfType<ShopItem>().ToList();

    public List<PackedScene> GetScenesToBuy() => itemsToBuy.ConvertAll((ShopItem item) => item.SceneToBuy);

    public int GetPrice()
    {
        int result = 0;

        foreach (ShopItem item in GetItems())
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
                .First((ShopItem item) => item.Name == name)
                .Show();
    }

    [TroughtEditor]
    private void OnBackPressed() => (Owner as Shop)?.CloseMenu();

    [TroughtEditor]
    private void OnBuyDicePressed()
    {
        if (Player.currentPlayer is null) return;

        var items = GetItems();
        var scenes = GetScenesToBuy();

        int price = GetPrice();

        if (Player.currentPlayer.Coins < price) return;

        foreach (var item in items) item.Sell();

        Dice dice = diceScene.Instance<Dice>();
        AddScenesToDice(dice, scenes);

        Player.currentPlayer.Coins -= price;
        Player.currentPlayer.AddDice(dice);

        EmitSignal(nameof(BoughtDice));
        Debug.LogU(this, "Selled Dice");
        DiceUpdate();
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
