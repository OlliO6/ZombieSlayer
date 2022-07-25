using Godot;
using System.Collections.Generic;
using System.Linq;

public class WeaponContainer : HBoxContainer
{
    [TroughtSignal]
    private void OnInventoryOpened() => UpdateWeaponFields();

    private void UpdateWeaponFields()
    {
        if (Player.currentPlayer is null) return;

        List<WeaponBase> weapons = Player.currentPlayer.GetWeapons().ToList();

        for (int i = 0; i < GetChildCount(); i++)
        {
            DragableWeaponField field = GetChild<DragableWeaponField>(i);

            field.Selected = false;

            if (i >= weapons.Count)
            {
                field.weapon = null;
                field.Icon = null;
                field.Disabled = true;
                continue;
            }

            WeaponBase weapon = weapons[i];

            field.weapon = weapon;
            field.Disabled = false;

            if (weapon is null)
            {
                field.Icon = null;
                continue;
            }

            field.Icon = weapon.icon;
        }
    }
}
