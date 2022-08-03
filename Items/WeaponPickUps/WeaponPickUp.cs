using System;
using Godot;

public class WeaponPickUp : InteractionPickupBase
{
    [Export] private PackedScene weaponScene;
    public override void Collect()
    {
        Player.currentPlayer.AddWeapon(weaponScene.Instance<WeaponBase>());
    }
}
