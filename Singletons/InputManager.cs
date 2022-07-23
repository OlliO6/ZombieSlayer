using Godot;
using System;

public class InputManager : Node
{
    public static InputManager instance;

    public static bool attackInput;

    [Signal] public delegate void OnAttackInputStarted();
    [Signal] public delegate void OnAttackInputEnded();

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
                EmitSignal(nameof(OnAttackInputStarted));
                return;
            }

            attackInput = false;
            EmitSignal(nameof(OnAttackInputEnded));
            return;
        }
    }
}
