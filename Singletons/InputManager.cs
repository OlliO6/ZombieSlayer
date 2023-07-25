using System;
using Additions;
using Godot;

public class InputManager : Control
{
    const string MoveLeftActionName = "MoveLeft";
    const string MoveRightActionName = "MoveRight";
    const string MoveUpActionName = "MoveUp";
    const string MoveDownActionName = "MoveDown";

    public enum InputType
    {
        MouseAndKeyboard,
        Controller
    }

    public static InputManager instance;

    public static bool attackInput;
    private static InputType _currentInputType;

    public static event Action<InputType> InputTypeChanged;
    public static event Action AttackInputStarted;
    public static event Action AttackInputEnded;
    public static event Action AbilityInputStarted;
    public static event Action AbilityInputEnded;
    public static event Action DropWeaponPressed;
    public static event Action UICancelPressed;
    public static event Action InventoryPressed;
    public static event Action DiceMenuPressed;
    public static event Action SwitchWeaponLeftPressed;
    public static event Action SwitchWeaponRightPressed;
    public static event Action PausePressed;
    public static event Action InteractPressed;

    private static readonly InputEventJoypadMotion MoveLeftControllerEvent = new() { Axis = (int)JoystickList.Axis0, AxisValue = -1 };
    private static readonly InputEventJoypadMotion MoveRightControllerEvent = new() { Axis = (int)JoystickList.Axis0, AxisValue = 1 };
    private static readonly InputEventJoypadMotion MoveUpControllerEvent = new() { Axis = (int)JoystickList.Axis1, AxisValue = -1 };
    private static readonly InputEventJoypadMotion MoveDownControllerEvent = new() { Axis = (int)JoystickList.Axis1, AxisValue = 1 };

    private Control _focusedControl;

    public static bool ProcessInput
    {
        get => instance.IsProcessingUnhandledInput();
        set => instance.SetProcessUnhandledInput(value);
    }

    public static InputType CurrentInputType
    {
        get => _currentInputType;
        set
        {
            if (value == CurrentInputType)
                return;

            _currentInputType = value;
            InputTypeChanged?.Invoke(value);

            switch (CurrentInputType)
            {
                case InputType.MouseAndKeyboard:
                    if (InputMap.ActionHasEvent(MoveLeftActionName, MoveLeftControllerEvent))
                        InputMap.ActionEraseEvent(MoveLeftActionName, MoveLeftControllerEvent);
                    if (InputMap.ActionHasEvent(MoveRightActionName, MoveRightControllerEvent))
                        InputMap.ActionEraseEvent(MoveRightActionName, MoveRightControllerEvent);
                    if (InputMap.ActionHasEvent(MoveUpActionName, MoveUpControllerEvent))
                        InputMap.ActionEraseEvent(MoveUpActionName, MoveUpControllerEvent);
                    if (InputMap.ActionHasEvent(MoveDownActionName, MoveDownControllerEvent))
                        InputMap.ActionEraseEvent(MoveDownActionName, MoveDownControllerEvent);
                    break;

                case InputType.Controller:
                    if (!InputMap.ActionHasEvent(MoveLeftActionName, MoveLeftControllerEvent))
                        InputMap.ActionAddEvent(MoveLeftActionName, MoveLeftControllerEvent);
                    if (!InputMap.ActionHasEvent(MoveRightActionName, MoveRightControllerEvent))
                        InputMap.ActionAddEvent(MoveRightActionName, MoveRightControllerEvent);
                    if (!InputMap.ActionHasEvent(MoveUpActionName, MoveUpControllerEvent))
                        InputMap.ActionAddEvent(MoveUpActionName, MoveUpControllerEvent);
                    if (!InputMap.ActionHasEvent(MoveDownActionName, MoveDownControllerEvent))
                        InputMap.ActionAddEvent(MoveDownActionName, MoveDownControllerEvent);
                    break;
            }
        }
    }

    public override void _Ready()
    {
        instance = this;
        PauseMode = PauseModeEnum.Process;
        CurrentInputType = InputType.MouseAndKeyboard;
        GD.Randomize();
    }

    public override void _Process(float delta)
    {
        var newFocus = GetFocusOwner();

        if (newFocus != _focusedControl)
        {
            _focusedControl = newFocus;
            if (newFocus is null)
                TryFocusButton();
        }
    }

    private void TryFocusButton()
    {
        foreach (var button in GetTree().Root.GetAllChildren<GameButton>())
            button.UpdateSelection();
    }

    public static void GetMovementInput(out Vector2 inputVector, out float lenght) => instance._GetMovementInput(out inputVector, out lenght);

    private void _GetMovementInput(out Vector2 inputVector, out float lenght)
    {
        if (!ProcessInput)
        {
            inputVector = Vector2.Zero;
            lenght = 0;
            return;
        }

        const float deadzone = 0.2f;

        var vector = new Vector2(
                Input.GetActionRawStrength(MoveRightActionName) - Input.GetActionRawStrength(MoveLeftActionName),
                Input.GetActionRawStrength(MoveDownActionName) - Input.GetActionRawStrength(MoveUpActionName));

        lenght = vector.Length().Clamp01();
        inputVector = lenght > deadzone ? (lenght is >= 1 ? vector.Normalized() : vector) : Vector2.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventJoypadMotion joypadMotion && joypadMotion.AxisValue.Abs() > 0.3f || @event is InputEventJoypadButton)
        {
            CurrentInputType = InputType.Controller;
            return;
        }

        if (@event is InputEventMouseButton or InputEventKey)
        {
            CurrentInputType = InputType.MouseAndKeyboard;
            return;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("Attack"))
        {
            if (@event.IsPressed())
            {
                attackInput = true;
                AttackInputStarted?.Invoke();
                return;
            }

            attackInput = false;
            AttackInputEnded?.Invoke();
            return;
        }

        if (@event.IsAction("Ability"))
        {
            if (@event.IsPressed())
            {
                AbilityInputStarted?.Invoke();
                return;
            }

            AbilityInputEnded?.Invoke();
            return;
        }

        InvokeIfActionPressed(@event, "Interact", InteractPressed);
        InvokeIfActionPressed(@event, "DropWeapon", DropWeaponPressed);
        InvokeIfActionPressed(@event, "SwitchWeaponRight", SwitchWeaponRightPressed);
        InvokeIfActionPressed(@event, "SwitchWeaponLeft", SwitchWeaponLeftPressed);
        InvokeIfActionPressed(@event, "Pause", PausePressed, true);
        InvokeIfActionPressed(@event, "ui_cancel", UICancelPressed, true);
        InvokeIfActionPressed(@event, "Inventory", InventoryPressed, true);
        InvokeIfActionPressed(@event, "DiceMenu", DiceMenuPressed, true);
    }

    private bool InvokeIfActionPressed(InputEvent @event, string actionName, Action eventAction, bool ignorePausing = false)
    {
        if (@event.IsActionPressed(actionName) && (ignorePausing || !instance.GetTree().Paused))
        {
            eventAction?.Invoke();
            return true;
        }
        return false;
    }

    public static void RotateWeapon(WeaponBase weapon, bool flip)
    {
        if (!ProcessInput)
            return;

        if (CurrentInputType is InputType.MouseAndKeyboard)
        {
            Vector2 mousePos = weapon.GetGlobalMousePosition();
            weapon.LookAt(mousePos);

            if (weapon.GlobalPosition.x > mousePos.x)
            {
                weapon.Scale = new Vector2(-1, 1);
                weapon.Rotate(Mathf.Deg2Rad(180));
                return;
            }
            weapon.Scale = new Vector2(1, 1);
            return;
        }

        const float deadzone = 0.3f;

        var vector = new Vector2(
                Input.GetActionRawStrength("AimRight") - Input.GetActionRawStrength("AimLeft"),
                Input.GetActionRawStrength("AimDown") - Input.GetActionRawStrength("AimUp"));

        if (vector.Length() < deadzone)
            return;

        weapon.Rotation = vector.Angle();
        if (vector.x < 0)
        {
            weapon.Scale = new Vector2(-1, 1);
            weapon.Rotate(Mathf.Deg2Rad(180));
            return;
        }
        weapon.Scale = new Vector2(1, 1);
        return;
    }
}
