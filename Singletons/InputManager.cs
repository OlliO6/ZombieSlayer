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
    public static event Action InteractPressed;

    public static bool ProcessInput
    {
        get => instance.IsProcessingUnhandledInput();
        set => instance.SetProcessUnhandledInput(value);
    }

    public override void _Ready()
    {
        instance = this;
        PauseMode = PauseModeEnum.Process;
        GD.Randomize();
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

        InvokeIfActionPressed(@event, "Interact", InteractPressed);
        InvokeIfActionPressed(@event, "DropWeapon", DropWeaponPressed);
        InvokeIfActionPressed(@event, "Pause", PausePressed, true);
        InvokeIfActionPressed(@event, "ui_cancel", UICancelPressed, true);
        InvokeIfActionPressed(@event, "Inventory", InventoryPressed, true);
        InvokeIfActionPressed(@event, "DiceMenu", DiceMenuPressed, true);

        if (@event is InputEventKey keyInput)
        {
            if (keyInput.Pressed && keyInput.Scancode is (uint)KeyList.F11)
            {
                ToggleFullscreen();
            }
        }
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

    private void ToggleFullscreen()
    {
        OptionsManager.IsFullscreen = !OptionsManager.IsFullscreen;
        OptionsManager.UpdateOptions();
    }
}
