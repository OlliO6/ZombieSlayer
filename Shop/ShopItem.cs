using Godot;
using System;

public class ShopItem : Control
{
    [Export] public PackedScene sceneToBuy;
    [Export] private int startPrice = 10;
    [Export] private int priceAdded = 10;
    [Export] private float priceMultiplier = 1.3f;

    [Signal] public delegate void OnCurrentAmountChanged();

    public int currentAmount;
    public int currentPrice;

    private int buyCount;

    private Label priceLabel;

    public int GetPrice()
    {
        return Mathf.RoundToInt(startPrice + (buyCount * priceAdded) * (priceMultiplier * buyCount));
    }

    public override void _Ready()
    {
        Connect(nameof(OnCurrentAmountChanged), Owner, "OnUpdateRatio");
        priceLabel = GetNode<Label>("Label");

        UpdateShopItem();
    }

    public void UpdateShopItem()
    {
        priceLabel.Text = GetPrice().ToString();

        if (currentAmount is 0)
            SetRemoveButtonEnabled(false);
        else
            SetRemoveButtonEnabled(true);

    }

    [TroughtSignal]
    private void OnAddButtonPressed()
    {
        currentAmount++;
        currentPrice += GetPrice();
        buyCount++;

        EmitSignal(nameof(OnCurrentAmountChanged));
        UpdateShopItem();
    }
    [TroughtSignal]
    private void OnRemoveButtonPressed()
    {
        currentAmount--;
        buyCount--;
        currentPrice -= GetPrice();

        EmitSignal(nameof(OnCurrentAmountChanged));
        UpdateShopItem();
    }

    public void SetAddButtonEnabled(bool enabled)
    {
        GetNode<Button>("HBoxContainer/AddButton").Disabled = !enabled;
    }
    public void SetRemoveButtonEnabled(bool enabled)
    {
        GetNode<Button>("HBoxContainer/RemoveButton").Disabled = !enabled;
    }

    public void Sell()
    {
        currentAmount = 0;
        currentPrice = 0;
    }
}
