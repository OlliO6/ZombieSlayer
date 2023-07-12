using System;
using Additions;
using Godot;

[Tool]
public abstract class ShopItem : Control
{
    [Signal] public delegate void Selled();

    [Export]
    public PackedScene SceneToBuy
    {
        get => _sceneToBuy;
        set
        {
            _sceneToBuy = value;

            if (Icons.IconPickupMatrix.ContainsKey(value))
                SetDeferred(nameof(Icon), Icons.IconPickupMatrix[value]);
        }
    }

    [Export]
    public Texture Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            iconRect?.SetDeferred("texture", value);
        }
    }

    private Texture _icon;
    private PackedScene _sceneToBuy;

    protected TextureRect iconRect;
    protected Label priceLabel;

    public abstract int GetPrice();

    public override void _Ready()
    {
        priceLabel = GetNode<Label>("%PriceLabel");
        iconRect = GetNode<TextureRect>("%Icon");
        UpdateShopItem();
        HintTooltip = SceneToBuy.GetDescriptionForScene();
    }

    public virtual void UpdateShopItem()
    {
        priceLabel.Text = GetPrice().ToString();
    }

    public virtual void Sell()
    {
        UpdateShopItem();
        EmitSignal(nameof(Selled));
    }
}
