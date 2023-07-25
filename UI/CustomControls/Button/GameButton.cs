using Godot;
using System;
using Additions;

public class GameButton : Button
{
    [Export] public bool autoSelect;

    public override void _Ready()
    {
        this.CenterPivotOffset();

        if (autoSelect)
        {
            UpdateSelection();
            Connect("visibility_changed", this, nameof(UpdateSelection));
            InputManager.InputTypeChanged += OnInputTypeChanged;
        }
    }

    public override void _ExitTree()
    {
        InputManager.InputTypeChanged -= OnInputTypeChanged;
    }

    private void OnInputTypeChanged(InputManager.InputType type)
    {
        if (type is InputManager.InputType.Controller)
            UpdateSelection();
    }

    public void UpdateSelection()
    {
        if (autoSelect && IsVisibleInTree())
        {
            var selectAudioPlayer = GetNode<RngAudioPlayer>("SelectAudio");
            selectAudioPlayer.mute = true;
            GrabFocus();
            selectAudioPlayer.mute = false;
        }
    }

    public override void _Process(float delta)
    {
        // If Mouse exists while pressing (Mouse exited signal doesn't work in this case)
        if (Pressed && Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            if (!GetGlobalRect().HasPoint(GetGlobalMousePosition()))
                animationPlayer.Play("Idle", 0.1f);
            else if (animationPlayer.CurrentAnimation != "Down")
                animationPlayer.Play("Down", 0.1f);
        }
    }
}
