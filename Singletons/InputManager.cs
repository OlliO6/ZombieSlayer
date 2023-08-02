using System;
using Additions;
using Godot;
using System.Linq;

public class InputManager : Control
{
    const string MoveLeftActionName = "MoveLeft";
    const string MoveRightActionName = "MoveRight";
    const string MoveUpActionName = "MoveUp";
    const string MoveDownActionName = "MoveDown";

    public const string DontStealFocusGroup = "DontStealFocus";

    public enum InputType
    {
        MouseAndKeyboard,
        Controller
    }

    public enum DeviceType
    {
        KeyboardMouse,
        Playstation,
        XBox,
        Nintendo
    }

    public static InputManager instance;

    private static InputType _currentInputType;

    public static event Action<bool> InputDisabled;
    public static event Action InputTypeChanged;
    public static event Action DeviceChanged;
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

    public static event Action AttackLeftStarted;
    public static event Action AttackLeftEnded;
    public static event Action AttackRightStarted;
    public static event Action AttackRightEnded;
    public static event Action AbilityRightStarted;
    public static event Action AbilityRightEnded;
    public static event Action AbilityLeftStarted;
    public static event Action AbilityLeftEnded;

    private static readonly InputEventJoypadMotion MoveLeftControllerEvent = new() { Axis = (int)JoystickList.Axis0, AxisValue = -1 };
    private static readonly InputEventJoypadMotion MoveRightControllerEvent = new() { Axis = (int)JoystickList.Axis0, AxisValue = 1 };
    private static readonly InputEventJoypadMotion MoveUpControllerEvent = new() { Axis = (int)JoystickList.Axis1, AxisValue = -1 };
    private static readonly InputEventJoypadMotion MoveDownControllerEvent = new() { Axis = (int)JoystickList.Axis1, AxisValue = 1 };

    private Control _focusedControl;
    private Control _focusStealer;
    private static DeviceType _currentDevice;

    public static bool AttackInput => Input.IsActionPressed("Attack") || Input.IsActionPressed("AttackLeft") || Input.IsActionPressed("AttackRight");
    public static bool AbilityInput => Input.IsActionPressed("Ability") || Input.IsActionPressed("AbilityLeft") || Input.IsActionPressed("AbilityRight");

    public static bool ProcessInput
    {
        get => instance.IsProcessingUnhandledInput();
        set
        {
            instance.SetProcessUnhandledInput(value);
            InputDisabled?.Invoke(value);
        }
    }

    public static DeviceType CurrentDevice
    {
        get => _currentDevice;
        set
        {
            if (value == CurrentDevice)
                return;

            _currentDevice = value;
            DeviceChanged?.Invoke();
            Debug.Log(instance, $"Device Changed to '{value}', name: '{Input.GetJoyName(0)}'");
        }
    }

    public static InputType CurrentInputType
    {
        get => _currentInputType;
        set
        {
            if (value == CurrentInputType)
                return;

            _currentInputType = value;
            InputTypeChanged?.Invoke();

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

            UpdateCurrentDeviceType();
        }
    }

    public override void _Ready()
    {
        instance = this;
        PauseMode = PauseModeEnum.Process;
        CurrentInputType = InputType.MouseAndKeyboard;
        GD.Randomize();

        _focusStealer = new()
        {
            FocusMode = FocusModeEnum.All,
            FocusNeighbourLeft = ".",
            FocusNeighbourRight = ".",
            FocusNeighbourTop = ".",
            FocusNeighbourBottom = "."
        };
        AddChild(_focusStealer);

        Input.Singleton.Connect("joy_connection_changed", this, nameof(OnJoyConnectionChanged));
        UpdateCurrentDeviceType();
    }

    private void OnJoyConnectionChanged(int device, bool connected)
    {
        if (device != 0)
            return;

        if (!connected)
        {
            CurrentInputType = InputType.MouseAndKeyboard;
            return;
        }

        CurrentInputType = InputType.Controller;
        UpdateCurrentDeviceType();
    }

    public static void UpdateCurrentDeviceType()
    {
        if (CurrentInputType is InputType.MouseAndKeyboard)
        {
            CurrentDevice = DeviceType.KeyboardMouse;
            return;
        }

        var controllerName = Input.GetJoyName(0).ToLowerInvariant();

        if (new[] { "ps3 controller", "ps4 controller", "ps5 controller" }.Any(c => controllerName.Contains(c)))
        {
            CurrentDevice = DeviceType.Playstation;
            return;
        }

        if (new[] { "switch controller", "switch pro controller", "joy-con", "nintendo switch" }.Any(c => controllerName.Contains(c)))
        {
            CurrentDevice = DeviceType.Nintendo;
            return;
        }

        CurrentDevice = DeviceType.XBox;
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

        if (!ProcessInput && (!_focusedControl?.IsInGroup(DontStealFocusGroup) ?? false))
        {
            _focusStealer.GrabFocus();
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
        bool playerProcessing = Player.currentPlayer?.PauseMode == PauseModeEnum.Process;

        InvokeIfActionPressed(@event, "Pause", PausePressed, true);
        InvokeIfActionPressed(@event, "ui_cancel", UICancelPressed, true);
        InvokeIfActionPressed(@event, "Inventory", InventoryPressed, true);
        InvokeIfActionPressed(@event, "DiceMenu", DiceMenuPressed, true);

        InvokeIfActionPressed(@event, "Interact", InteractPressed, playerProcessing);
        InvokeIfActionPressed(@event, "SwitchWeaponRight", SwitchWeaponRightPressed, playerProcessing);
        InvokeIfActionPressed(@event, "SwitchWeaponLeft", SwitchWeaponLeftPressed, playerProcessing);

        InvokeIfActionPressed(@event, "Attack", AttackInputStarted, playerProcessing);
        InvokeIfActionReleased(@event, "Attack", AttackInputEnded, playerProcessing);
        InvokeIfActionPressed(@event, "Ability", AbilityInputStarted, playerProcessing);
        InvokeIfActionReleased(@event, "Ability", AbilityInputEnded, playerProcessing);

        InvokeIfActionPressed(@event, "AttackRight", AttackRightStarted, playerProcessing);
        InvokeIfActionReleased(@event, "AttackRight", AttackRightEnded, playerProcessing);
        InvokeIfActionPressed(@event, "AttackLeft", AttackLeftStarted, playerProcessing);
        InvokeIfActionReleased(@event, "AttackLeft", AttackLeftEnded, playerProcessing);

        InvokeIfActionPressed(@event, "AbilityRight", AbilityRightStarted, playerProcessing);
        InvokeIfActionReleased(@event, "AbilityRight", AbilityRightEnded, playerProcessing);
        InvokeIfActionPressed(@event, "AbilityLeft", AbilityLeftStarted, playerProcessing);
        InvokeIfActionReleased(@event, "AbilityLeft", AbilityLeftEnded, playerProcessing);
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

    private bool InvokeIfActionReleased(InputEvent @event, string actionName, Action eventAction, bool ignorePausing = false)
    {
        if (@event.IsActionReleased(actionName) && (ignorePausing || !instance.GetTree().Paused))
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
