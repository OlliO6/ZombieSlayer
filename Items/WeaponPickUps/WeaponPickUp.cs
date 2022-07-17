using Godot;
using System;

public class WeaponPickUp : PickupBase
{
    [Export] private PackedScene weaponScene;
    public override void Collect()
    {
        Player.currentPlayer.ChangeWeapon(weaponScene);
    }
}
