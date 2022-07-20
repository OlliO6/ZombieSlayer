using Godot;
using Additions;
using System;

public class WeaponSwitcher : Node2D // TODO a visualisation of the weapon inventory
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

    [Signal] public delegate void OnWeaponChanged(int to);

    public override void _Ready()
    {
        if (GetChildCount() > 0)
        {
            foreach (WeaponBase weapon in this.GetChildren<WeaponBase>())
            {
                weapon.Disable();
            }

            currentIndex = 0;
            IndexChanged();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed()) return;

        if (@event is InputEventMouseButton mouseButtonInput)
        {
            if (mouseButtonInput.ButtonIndex is (int)ButtonList.WheelUp)
            {
                GD.Print("WheelUp");
                currentIndex++;

                if (currentIndex > GetChildCount() - 1) currentIndex = 0;

                IndexChanged();
                return;
            }
            if (mouseButtonInput.ButtonIndex is (int)ButtonList.WheelDown)
            {
                GD.Print("WheelDown");

                currentIndex--;

                if (currentIndex < 0) currentIndex = GetChildCount() - 1;

                IndexChanged();
            }
            return;
        }

        if (@event is InputEventKey keyInput)
        {
            if (keyInput.PhysicalScancode is (int)KeyList.Q)
            {
                DropWeapon(CurrentWeapon);
                return;
            }

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

            if (prevIndex != currentIndex) IndexChanged();
        }
    }

    public void IndexChanged()
    {
        GD.Print(currentIndex);

        CurrentWeapon = GetChild<WeaponBase>(currentIndex);

        EmitSignal(nameof(OnWeaponChanged), currentIndex);
    }

    public void AddWeapon(WeaponBase weapon)
    {
        AddChild(weapon);

        if (GetChildCount() > 9)
        {
            DropWeapon(CurrentWeapon);
        }

        currentIndex = GetChildCount() - 1;
        IndexChanged();
    }

    public void DropWeapon(WeaponBase weapon)
    {
        if (weapon.weaponPickup is not null)
        {
            WeaponPickUp pickup = GD.Load<PackedScene>(weapon.weaponPickup).Instance<WeaponPickUp>();

            GetTree().CurrentScene.AddChild(pickup);

            pickup.GlobalPosition = GlobalPosition;
        }

        int childCount = GetChildCount();

        if (currentIndex > childCount - 2) currentIndex = childCount - 2;

        RemoveChild(weapon);
        weapon.QueueFree();
        IndexChanged();
    }
}
