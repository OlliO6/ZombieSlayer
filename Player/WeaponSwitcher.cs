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
        InputManager.SwitchWeaponLeftPressed += SwitchWeaponLeft;
        InputManager.SwitchWeaponRightPressed += SwitchWeaponRight;
        InputManager.AttackInputStarted += AttackInputStarted;
        InputManager.AttackInputEnded += AttackInputEnded;
        InputManager.AttackLeftStarted += AttackLeftStarted;
        InputManager.AttackLeftEnded += AttackLeftEnded;
        InputManager.AttackRightStarted += AttackRightStarted;
        InputManager.AttackRightEnded += AttackRightEnded;
    }

    public override void _ExitTree()
    {
        InputManager.SwitchWeaponLeftPressed -= SwitchWeaponLeft;
        InputManager.SwitchWeaponRightPressed -= SwitchWeaponRight;
        InputManager.AttackInputStarted -= AttackInputStarted;
        InputManager.AttackInputEnded -= AttackInputEnded;
        InputManager.AttackLeftStarted -= AttackLeftStarted;
        InputManager.AttackLeftEnded -= AttackLeftEnded;
        InputManager.AttackRightStarted -= AttackRightStarted;
        InputManager.AttackRightEnded -= AttackRightEnded;
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

    private void AttackInputStarted()
    {
        CurrentWeapon?.AttackInputStarted();
    }

    private void AttackInputEnded()
    {
        CurrentWeapon?.AttackInputEnded();
    }

    private void AttackLeftStarted()
    {
        SwitchWeaponLeft();
        CurrentWeapon?.AttackInputStarted();
    }

    private void AttackRightStarted()
    {
        SwitchWeaponRight();
        CurrentWeapon?.AttackInputStarted();
    }

    private void AttackLeftEnded()
    {
        if (currentIndex == 0)
            CurrentWeapon?.AttackInputEnded();
    }

    private void AttackRightEnded()
    {
        if (currentIndex == 1)
            CurrentWeapon?.AttackInputEnded();
    }

    public override void _Process(float delta)
    {
        if (InputManager.AttackInput)
            CurrentWeapon?.AttackInputProcess(delta);
    }

    public void SwitchWeaponLeft()
    {
        if (currentIndex == 0)
            return;

        currentIndex = 0;
        WeaponsChanged();
    }

    public void SwitchWeaponRight()
    {
        if (currentIndex == 1)
            return;

        currentIndex = 1;
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
            }

            if (prevIndex != currentIndex) WeaponsChanged();
        }
    }

    public void WeaponsChanged()
    {
        if (currentIndex < 0)
            currentIndex = 0;
        else if (currentIndex > Mathf.Min(GetChildCount() - 1, 1))
            currentIndex = GetChildCount() - 1;

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
