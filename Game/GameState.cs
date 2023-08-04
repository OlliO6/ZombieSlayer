using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class GameState : YSort
{
    public event Action ShopUnlocked;
    public event Action InventoryUnlocked;
    public event Action HealthUnlocked;

    public static GameState instance;
    public static bool HasInstance => IsInstanceValid(instance);

    public GameStats stats = new GameStats();
    [Export] private NodePath shopParent;
    public Shop shop;
    public Player player;

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _ExitTree()
    {
        if (instance == this) instance = null;
    }

    public override void _Ready()
    {
        ExplanationsManager.StartExplanation("MoveAndAttack");
        player = GetNode<Player>("Player");
        player.WeaponInv.Connect(nameof(WeaponSwitcher.WeaponAdded), this, nameof(PlayerGotWeapon));
    }

    private void PlayerGotWeapon()
    {
        if (player.GetWeapons().Count() == 2)
        {
            ExplanationsManager.StartExplanation("SwitchWeapons");
            return;
        }
    }

    public void UnlockHealth()
    {
        stats.healthUnlocked = true;
        HealthUnlocked?.Invoke();
    }

    public void UnlockInventory()
    {
        stats.inventoryUnlocked = true;
        InventoryUnlocked?.Invoke();
    }

    public void UnlockShop()
    {
        shop = GD.Load<PackedScene>("res://Shop/Shop.tscn").Instance<Shop>();
        GetNode(shopParent).AddChild(shop);

        foreach (string shopItem in stats.unlockedShopItems)
            shop.UnlockShopItem(shopItem);

        ShopUnlocked?.Invoke();
    }

    public void UnlockShopItem(string shopItem)
    {
        stats.unlockedShopItems.Add(shopItem);

        if (IsInstanceValid(shop))
        {
            shop.UnlockShopItem(shopItem);
        }
    }

    public void UnlockWeaponEvolutions()
    {
        Debug.Log(this, "Weapon Evolutions Unlocked.");
    }
}

public struct GameStats
{
    public static GameStats Current => GameState.instance is null ? new() : GameState.instance.stats;

    public GameStats() { }

    public bool inventoryUnlocked = false, shopUnlocked = false, healthUnlocked = false;
    public List<string> unlockedShopItems = new();
}
