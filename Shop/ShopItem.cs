using Godot;
using System;
using Additions;

[Tool]
public class ShopItem : Control
{
    [Export] private int startPrice = 10;
    [Export] private int priceAdded = 10;
    [Export] private float priceMultiplier = 1.3f;

    [Export]
    public PackedScene SceneToBuy
    {
        get => _sceneToBuy;
        set
        {
            _sceneToBuy = value;

            if (Icons.IconPickupMatrix.ContainsKey(value)) SetDeferred(nameof(Icon), Icons.IconPickupMatrix[value]);
        }
    }

    [Export]
    private Texture Icon
    {
        get => _icon;
        set
        {
            GD.Print(value, IconRect);
            _icon = value;
            IconRect?.SetDeferred("texture", value);
        }
    }

    [Signal] public delegate void OnCurrentAmountChanged();

    public int currentAmount;
    public int currentPrice;

    #region IconRect Reference

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "HBoxContainer/Icon");

    #endregion

    private int buyCount;
    private Texture _icon;
    private PackedScene _sceneToBuy;

    private Label priceLabel;

    public int GetPrice()
    {
        return Mathf.RoundToInt(startPrice + (buyCount * priceAdded) * (priceMultiplier * buyCount));
    }

    public override void _Ready()
    {
        if (Owner is not null) Connect(nameof(OnCurrentAmountChanged), Owner, "OnUpdateRatio");
        priceLabel = GetNode<Label>("HBoxContainer/Label");

        UpdateShopItem();

        HintTooltip = GetDescription();
    }

    private string GetDescription()
    {
        if (SceneToBuy is null) return "";

        SceneState sceneState = SceneToBuy.GetState();

        int propCount = sceneState.GetNodePropertyCount(0);

        if (propCount > 1 && sceneState.GetNodePropertyValue(0, 1) is CSharpScript script)
        {
            return script.Call("GetDescription") as string;
        }

        return "";
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
