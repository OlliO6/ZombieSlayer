using System;
using Additions;
using Godot;

public class InputManager : Node
{
    public static InputManager instance;

    public static bool attackInput;

    public static event Action AttackInputStarted;
    public static event Action AttackInputEnded;
    public static event Action DropWeaponPressed;
    public static event Action UICancelPressed;
    public static event Action InventoryPressed;
    public static event Action DiceMenuPressed;
    public static event Action PausePressed;

    public override void _Ready()
    {
        instance = this;
        PauseMode = PauseModeEnum.Process;
        GD.Randomize();
    }

    public static void GetMovementInput(out Vector2 inputVector, out float lenght) => instance._GetMovementInput(out inputVector, out lenght);

    private void _GetMovementInput(out Vector2 inputVector, out float lenght)
    {
        const float deadzone = 0.2f;

        var vector = new Vector2(
                Input.GetActionRawStrength("MoveRight") - Input.GetActionRawStrength("MoveLeft"),
                Input.GetActionRawStrength("MoveDown") - Input.GetActionRawStrength("MoveUp"));

        lenght = vector.Length().Clamp01();
        inputVector = lenght > deadzone ? (lenght is >= 1 ? vector.Normalized() : vector) : Vector2.Zero;
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

        InvokeIfActionPressed(@event, "DropWeapon", DropWeaponPressed);
        InvokeIfActionPressed(@event, "Pause", PausePressed);
        InvokeIfActionPressed(@event, "ui_cancel", UICancelPressed);
        InvokeIfActionPressed(@event, "Inventory", InventoryPressed);
        InvokeIfActionPressed(@event, "DiceMenu", DiceMenuPressed);

        if (@event is InputEventKey keyInput)
        {
            if (keyInput.Pressed && keyInput.Scancode is (uint)KeyList.F11)
            {
                ToggleFullscreen();
            }
        }
    }
    private bool InvokeIfActionPressed(InputEvent @event, string actionName, Action eventAction)
    {
        if (@event.IsActionPressed(actionName))
        {
            eventAction?.Invoke();
            return true;
        }
        return false;
    }

    private void ToggleFullscreen()
    {
        OptionsManager.IsFullscreen = !OptionsManager.IsFullscreen;
        OptionsManager.UpdateOptions();
    }
}
