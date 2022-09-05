namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class ShopItem : LevelBuff
{
    [Export] public string itemName = "";

    public override void Apply()
    {
        (GetTree().CurrentScene as GameState).UnlockShopItem(itemName);
    }

    public override string GetBuffText() => $"Unlocked {itemName} (shop)";
}
