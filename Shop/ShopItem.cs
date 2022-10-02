using System;
using Additions;
using Godot;

[Tool]
public class ShopItem : Control
{
    [Signal] public delegate void Added(ShopItem item);
    [Signal] public delegate void Removed(ShopItem item);

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
            _icon = value;
            IconRect?.SetDeferred("texture", value);
        }
    }

    public int currentAmount;
    public int count;

    #region IconRect Reference

    private TextureRect storerForIconRect;
    public TextureRect IconRect => this.LazyGetNode(ref storerForIconRect, "HBoxContainer/Icon");

    #endregion

    private Texture _icon;
    private PackedScene _sceneToBuy;

    private Label priceLabel;

    public int GetPrice(int? count = null) => Mathf.RoundToInt((startPrice + priceAdded * (count is null ? this.count : count.Value)) * priceMultiplier);

    public override void _Ready()
    {
        if (Owner is not null)
        {
            Connect(nameof(Added), Owner, nameof(ShopMenu.OnItemAdded));
            Connect(nameof(Removed), Owner, nameof(ShopMenu.OnItemRemoved));
        }
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

        SetRemoveButtonEnabled(currentAmount is 0 ? false : true);
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

    public void Sell()
    {
        currentAmount = 0;
    }
}
