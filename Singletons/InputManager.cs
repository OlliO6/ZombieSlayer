using Godot;
using System;

public class InputManager : Node
{
    public static InputManager instance;

    public static bool attackInput;

    [Signal] public delegate void AttackInputStarted();
    [Signal] public delegate void AttackInputEnded();

    public override void _Ready()
    {
        instance = this;
        PauseMode = PauseModeEnum.Process;
        GD.Randomize();
    }

    public static Vector2 GetMovementInput() => instance._GetMovementInput();

    private Vector2 _GetMovementInput()
    {
        const float deadzone = 0.2f;

        return Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown", deadzone);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("Attack"))
        {
            if (@event.IsPressed())
            {
                attackInput = true;
                EmitSignal(nameof(AttackInputStarted));
                return;
            }

            attackInput = false;
            EmitSignal(nameof(AttackInputEnded));
            return;
        }

        if (@event is InputEventKey keyInput)
        {
            if (keyInput.Pressed)
            {
                if (keyInput.Scancode is (uint)KeyList.F11)
                {
                    ToggleFullscreen();
                    return;
                }

                return;
            }

            return;
        }
    }

    private void ToggleFullscreen()
    {
        OptionsManager.fullscreen = !OptionsManager.fullscreen;

        OptionsManager.UpdateOptions();
    }
}
