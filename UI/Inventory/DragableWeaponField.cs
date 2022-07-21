using Godot;
using System;

public class DragableWeaponField : WeaponField
{
    public WeaponBase weapon;

    public bool Disabled
    {
        get => disabled;
        set
        {
            disabled = value;

            Modulate = value ? new Color(Colors.White, 0.5f) : Colors.White;
        }
    }
    private bool disabled;

    public class WeaponData : Godot.Object
    {
        public WeaponBase weapon;
        public int fromIndex;

        public WeaponData(WeaponBase weaponBase, int index)
        {
            this.weapon = weaponBase;
            this.fromIndex = index;
        }
    }

    public override bool CanDropData(Vector2 position, object data)
    {
        if (Disabled || data is not WeaponData weaponData) return false;

        return true;
    }

    public override object GetDragData(Vector2 position)
    {
        if (weapon is null) return null;

        CenterContainer preview = new CenterContainer();
        TextureRect previewRect = new TextureRect() { Texture = weapon.icon };
        preview.AddChild(previewRect);
        preview.UseTopLeft = true;

        SetDragPreview(preview);
        return new WeaponData(weapon, Index);
    }

    public override void DropData(Vector2 position, object data)
    {
        WeaponData weaponData = data as WeaponData;

        if (weaponData is null) return;

        MovePlayerWeapon(weaponData.fromIndex, Index);

        DragableWeaponField cameFrom = GetParent().GetChild<DragableWeaponField>(weaponData.fromIndex);

        cameFrom.weapon = weapon;
        cameFrom.Icon = Icon;

        weapon = weaponData.weapon;
        Icon = weaponData.weapon.icon;
    }

    private void MovePlayerWeapon(int fromIndex, int toIndex)
    {
        if (Player.currentPlayer is null) return;

        WeaponSwitcher weapons = Player.currentPlayer.Weapons;

        weapons.MoveChild(weapons.GetChild(fromIndex), toIndex);

        weapons.WeaponsChanged();
    }
}
