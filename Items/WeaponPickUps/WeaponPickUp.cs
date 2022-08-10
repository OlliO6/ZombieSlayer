using System;
using Godot;

public class WeaponPickUp : InteractionPickupBase
{
    [Export] private PackedScene weaponScene;
    public override void Collect()
    {
        GD.Print(weaponScene.Instance().GetType().ToString());
        Player.currentPlayer.AddWeapon(weaponScene.Instance<WeaponBase>());
    }
}
