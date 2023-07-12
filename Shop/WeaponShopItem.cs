using System;
using Godot;

[Tool]
public class WeaponShopItem : ShopItem
{
    [Signal] public delegate void Selected(WeaponShopItem item);

    [Export] public int price;

    public override int GetPrice() => price;

    public override void _Ready()
    {
        base._Ready();

        Connect("pressed", this, nameof(Select));

        if (Owner is not null)
        {
            Connect(nameof(Selected), Owner, nameof(WeaponShop.SelectItem));
        }
    }

    public void Select()
    {
        EmitSignal(nameof(Selected), this);
    }

    public void Deselect()
    {
        Set("pressed", false);
    }
}
