using System.Collections.Generic;
using Godot;

[Tool]
public static class Icons
{
    /// <summary>Every icon should be 16 by 16 pixels.</summary>
    public static Dictionary<PackedScene, Texture> IconPickupMatrix => iconPickupMatrix;
    private static Dictionary<PackedScene, Texture> iconPickupMatrix = new()
    {
        // Upgrade Pickups
        {GD.Load<PackedScene>("res://Items/UpgradePickups/SpeedPickUp.tscn"), GD.Load<Texture>("res://Player/Upgrades/Icons/SpeedUpgrade.png")},
        {GD.Load<PackedScene>("res://Items/UpgradePickups/MagnetPickup.tscn"), GD.Load<Texture>("res://Player/Upgrades/Icons/Magnet.png")},
        {GD.Load<PackedScene>("res://Items/UpgradePickups/DamagePickUp.tscn"), GD.Load<Texture>("res://Player/Upgrades/Icons/DamageUp.png")},
        // Weapon Pickups
        {GD.Load<PackedScene>("res://Items/WeaponPickUps/PistolPickup.tscn"), GD.Load<Texture>("res://Weapons/Guns/Pistol/PistolIcon.png")},
        {GD.Load<PackedScene>("res://Items/WeaponPickUps/RiflePickup.tscn"), GD.Load<Texture>("res://Weapons/Guns/Rifle/RifleIcon.png")},
        {GD.Load<PackedScene>("res://Items/WeaponPickUps/SwordPickup.tscn"), GD.Load<Texture>("res://Weapons/Melee/Sword/SwordIcon.png")},
        // Other Pickups
        {GD.Load<PackedScene>("res://Items/Health/HealthPickup.tscn"), GD.Load<Texture>("res://Items/Health/HealthIcon.png")},
    };
}
