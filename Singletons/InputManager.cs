using Godot;
using System;

public class InputManager : Node
{
    public static InputManager instance;

    public override void _Ready()
    {
        instance = this;
        GD.Randomize();
    }

    public static Vector2 GetMovementInput() => instance._GetMovementInput();

    private Vector2 _GetMovementInput()
    {
        const float deadzone = 0.2f;

        return Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown", deadzone);
    }
}
