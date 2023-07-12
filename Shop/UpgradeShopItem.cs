using System;
using Additions;
using Godot;

[Tool]
public class UpgradeShopItem : ShopItem
{
    [Signal] public delegate void Added(UpgradeShopItem item);
    [Signal] public delegate void Removed(UpgradeShopItem item);

    [Export] private int startPrice = 10;
    [Export] private int priceAdded = 10;
    [Export] private float priceMultiplier = 1.3f;

    public int currentAmount;
    public int count;

    public override int GetPrice() => Mathf.RoundToInt(startPrice + priceAdded * this.count * priceMultiplier);
    public int GetPrice(int count) => Mathf.RoundToInt((startPrice + priceAdded * count) * priceMultiplier);

    public override void _Ready()
    {
        base._Ready();

        if (Owner is not null)
        {
            Connect(nameof(Added), Owner, nameof(UpgradeShop.OnItemAdded));
            Connect(nameof(Removed), Owner, nameof(UpgradeShop.OnItemRemoved));
        }
    }

    public override void UpdateShopItem()
    {
        base.UpdateShopItem();
        SetRemoveButtonEnabled(currentAmount <= 0 ? false : true);
    }

    [TroughtEditor]
    private void OnAddButtonPressed()
    {
        currentAmount++;
        count++;

        EmitSignal(nameof(Added), this);
        UpdateShopItem();
    }

    [TroughtEditor]
    private void OnRemoveButtonPressed()
    {
        currentAmount--;
        count--;

        EmitSignal(nameof(Removed), this);
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

    public override void Sell()
    {
        currentAmount = 0;
        base.Sell();
    }
}
