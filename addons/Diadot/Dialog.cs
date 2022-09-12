namespace Diadot;

using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class Dialog : Node, IDialogProvider
{
    [Export(PropertyHint.MultilineText)] private string text = "";

    public bool waitingForInput;
    public event Action<IDialogProvider> Ended;

    public string Text { get => text; set => text = value; }

    public void OnTextAdvanced()
    {
    }

    public void OnTextFinished()
    {
        waitingForInput = true;
    }

    private void InputAwaited()
    {
        Ended?.Invoke(this);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (waitingForInput && @event.IsActionPressed(ProjectSettingsControl.SkipInput))
        {
            InputAwaited();
        }
    }
}
