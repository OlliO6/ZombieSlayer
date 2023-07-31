using Godot;
using System;

[Tool]
public class ButtonIcons : Node2D
{
    public enum InputAction
    {
        Move,
        Aim,
        Attack,
        Ability,
        Interact,
        DiceMenu,
        Inventory,
        SwitchWeaponLeft,
        SwitchWeaponRight
    }

    [Export] public InputAction inputAction;

#if TOOLS
    [Export]
    public InputManager.DeviceType testDevice;
    [Export]
    public bool TestUpdate
    {
        get => false;
        set => UpdateIcon();
    }
#endif

    public Sprite UnknownIcon => GetNode<Sprite>("Unknown");
    public Sprite WasdIcon => GetNode<Sprite>("Wasd");
    public Sprite MouseCursorIcon => GetNode<Sprite>("MouseCursor");
    public Sprite MouseIcons => GetNode<Sprite>("Mouse");
    public Sprite StickIcons => GetNode<Sprite>("Sticks");
    public Sprite DPadIcons => GetNode<Sprite>("DPad");
    public Sprite KeyboardIcons => GetNode<Sprite>("Keyboard");
    public Sprite ControllerIcons => GetNode<Sprite>("Controller");

    private static Texture _playstationControllerIcons = GD.Load<Texture>("res://UI/ButtonIcons/Controller/Playstation.png");
    private static Texture _xboxControllerIcons = GD.Load<Texture>("res://UI/ButtonIcons/Controller/XBox.png");
    private static Texture _nintendoControllerIcons = GD.Load<Texture>("res://UI/ButtonIcons/Controller/Nintendo.png");

    public override void _EnterTree()
    {
        InputManager.DeviceChanged += UpdateIcon;
    }

    public override void _ExitTree()
    {
        InputManager.DeviceChanged -= UpdateIcon;
    }

    public override void _Ready()
    {
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        var device = InputManager.CurrentDevice;

        if (Engine.EditorHint)
            device = testDevice;

        HideIconSprites();

        if (device is InputManager.DeviceType.KeyboardMouse)
        {
            switch (inputAction)
            {
                case InputAction.Move:
                    WasdIcon.Show();
                    return;

                case InputAction.Aim:
                    MouseCursorIcon.Show();
                    return;

                case InputAction.Attack:
                    MouseIcons.Show();
                    MouseIcons.Frame = 0;
                    return;

                case InputAction.Ability:
                    MouseIcons.Show();
                    MouseIcons.Frame = 1;
                    return;

                case InputAction.Interact:
                    KeyboardIcons.Show();
                    KeyboardIcons.Frame = 2;
                    return;

                case InputAction.DiceMenu:
                    KeyboardIcons.Show();
                    KeyboardIcons.Frame = 0;
                    return;

                case InputAction.Inventory:
                    KeyboardIcons.Show();
                    KeyboardIcons.Frame = 1;
                    return;

                case InputAction.SwitchWeaponLeft:
                    KeyboardIcons.Show();
                    KeyboardIcons.Frame = 3;
                    return;

                case InputAction.SwitchWeaponRight:
                    KeyboardIcons.Show();
                    KeyboardIcons.Frame = 4;
                    return;
            }
            return;
        }

        switch (inputAction)
        {
            case InputAction.Move:
                StickIcons.Show();
                StickIcons.Frame = 0;
                return;

            case InputAction.Aim:
                StickIcons.Show();
                StickIcons.Frame = 1;
                return;

            case InputAction.Inventory:
                DPadIcons.Show();
                DPadIcons.Frame = 0;
                return;

            case InputAction.DiceMenu:
                DPadIcons.Show();
                DPadIcons.Frame = 1;
                return;
        }

        ControllerIcons.Show();
        ControllerIcons.Texture = device switch
        {
            InputManager.DeviceType.Playstation => _playstationControllerIcons,
            InputManager.DeviceType.Nintendo => _nintendoControllerIcons,
            _ => _xboxControllerIcons
        };

        switch (inputAction)
        {
            case InputAction.Attack:
                ControllerIcons.Frame = 6;
                return;

            case InputAction.Ability:
                ControllerIcons.Frame = 7;
                return;

            case InputAction.Interact:
                ControllerIcons.Frame = 0;
                return;

            case InputAction.SwitchWeaponLeft:
                ControllerIcons.Frame = 5;
                return;

            case InputAction.SwitchWeaponRight:
                ControllerIcons.Frame = 4;
                return;
        }
    }

    private void HideIconSprites()
    {
        UnknownIcon.Hide();
        WasdIcon.Hide();
        MouseCursorIcon.Hide();
        MouseIcons.Hide();
        StickIcons.Hide();
        DPadIcons.Hide();
        KeyboardIcons.Hide();
        ControllerIcons.Hide();
    }
}
