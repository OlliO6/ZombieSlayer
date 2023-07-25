using System;
using Additions;
using Godot;

public class WeaponSwitcher : Node2D
{
    public int currentIndex;

    public WeaponBase CurrentWeapon
    {
        get => _currentWeapon;
        set
        {
            if (IsInstanceValid(CurrentWeapon)) CurrentWeapon.Disable();
            _currentWeapon = value;
            CurrentWeapon?.Enable();
        }
    }
    private WeaponBase _currentWeapon;

    [Signal] public delegate void WeaponChanged(int to);

    public override void _EnterTree()
    {
        InputManager.DropWeaponPressed += DropCurrentWeapon;
        InputManager.SwitchWeaponLeftPressed += SwitchWeaponLeft;
        InputManager.SwitchWeaponRightPressed += SwitchWeaponRight;
    }

    public override void _ExitTree()
    {
        InputManager.DropWeaponPressed -= DropCurrentWeapon;
        InputManager.SwitchWeaponLeftPressed -= SwitchWeaponLeft;
        InputManager.SwitchWeaponRightPressed -= SwitchWeaponRight;
    }

    public override void _Ready()
    {
        if (GetChildCount() > 0)
        {
            foreach (WeaponBase weapon in this.GetChildren<WeaponBase>())
            {
                weapon.Disable();
            }

            currentIndex = 0;
            WeaponsChanged();
        }
    }

    private void SwitchWeaponLeft()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = GetChildCount() - 1;
        WeaponsChanged();
    }

    private void SwitchWeaponRight()
    {
        currentIndex++;
        if (currentIndex > GetChildCount() - 1) currentIndex = 0;
        WeaponsChanged();
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed())
            return;

        if (@event is InputEventKey keyInput)
        {
            int prevIndex = currentIndex;

            switch (keyInput.PhysicalScancode)
            {
                case (int)KeyList.Key1:
                    if (GetChildCount() >= 1) currentIndex = 0;
                    break;
                case (int)KeyList.Key2:
                    if (GetChildCount() >= 2) currentIndex = 1;
                    break;
                case (int)KeyList.Key3:
                    if (GetChildCount() >= 3) currentIndex = 2;
                    break;
                case (int)KeyList.Key4:
                    if (GetChildCount() >= 4) currentIndex = 3;
                    break;
                case (int)KeyList.Key5:
                    if (GetChildCount() >= 5) currentIndex = 4;
                    break;
                case (int)KeyList.Key6:
                    if (GetChildCount() >= 6) currentIndex = 5;
                    break;
                case (int)KeyList.Key7:
                    if (GetChildCount() >= 7) currentIndex = 6;
                    break;
                case (int)KeyList.Key8:
                    if (GetChildCount() >= 8) currentIndex = 7;
                    break;
                case (int)KeyList.Key9:
                    if (GetChildCount() >= 9) currentIndex = 8;
                    break;
            }

            if (prevIndex != currentIndex) WeaponsChanged();
        }
    }

    public void WeaponsChanged()
    {
        Debug.Log(this, $"Switched weapon to {(IsInstanceValid(CurrentWeapon) ? CurrentWeapon.Name : CurrentWeapon)}");

        CurrentWeapon = GetChild<WeaponBase>(currentIndex);

        EmitSignal(nameof(WeaponChanged), currentIndex);
    }

    public void AddWeapon(WeaponBase weapon)
    {
        Debug.LogU(this, $"Added weapon {weapon.Name}");

        AddChild(weapon);
        MoveChild(weapon, currentIndex);

        if (GetChildCount() > 9)
        {
            DropWeapon(CurrentWeapon);
        }

        WeaponsChanged();
    }

    public void DropCurrentWeapon() => CallDeferred(nameof(DropWeapon), CurrentWeapon);

    public void DropWeapon(WeaponBase weapon)
    {
        if (weapon is null) return;

        if (GetChildCount() <= 1) return;

        if (weapon.weaponPickupScene is not null)
        {
            WeaponPickUp pickup = weapon.weaponPickupScene.Instance<WeaponPickUp>();

            GetTree().CurrentScene.AddChild(pickup);

            pickup.GlobalPosition = GlobalPosition;
        }

        int childCount = GetChildCount();

        if (currentIndex > childCount - 2) currentIndex = childCount - 2;

        RemoveChild(weapon);
        weapon.QueueFree();
        WeaponsChanged();

        Debug.Log(this, $"Dropped weapon '{weapon.Filename.GetFile().BaseName()}'");
    }
}
