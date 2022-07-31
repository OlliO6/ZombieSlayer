using Additions;
using Godot;

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
        OptionsManager.IsFullscreen = !OptionsManager.IsFullscreen;
        OptionsManager.UpdateOptions();
    }
}
