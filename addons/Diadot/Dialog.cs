namespace Diadot;

using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class Dialog : Node, IDialogProvider
{
    [Export(PropertyHint.MultilineText)] private string text = "";

    public bool waitingForInput;
    public event Action<IDialogProvider, NodePath> Ended;
    public event Action<IDialogProvider> DialogChanged;

    public string Text { get => text; set => text = value; }

    public void OnTextAdvanced() { }

    public void OnTextFinished()
    {
        waitingForInput = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (waitingForInput && @event.IsActionPressed(ProjectSettingsControl.SkipInput))
        {
            Finish();
        }
    }

    public void OnStarted()
    {
        waitingForInput = false;

        foreach (IDialogProvider dialogChild in this.GetChildren<IDialogProvider>())
        {
            dialogChild.Ended -= OnSubDialogEnded;
            dialogChild.DialogChanged -= OnSubDialogChanged;
        }
    }

    public void Finish()
    {
        waitingForInput = false;

        if (GetChildCount() is 0)
        {
            Ended?.Invoke(this, Name);
            return;
        }

        IDialogProvider dialog = GetChild<IDialogProvider>(0);
        dialog.DialogChanged += OnSubDialogChanged;
        dialog.Ended += OnSubDialogEnded;
        DialogChanged?.Invoke(dialog);
    }

    private void OnSubDialogEnded(IDialogProvider sender, NodePath dialog)
    {
        sender.Ended -= OnSubDialogEnded;
        sender.DialogChanged -= OnSubDialogChanged;
        Ended?.Invoke(this, dialog + Name);
    }

    private void OnSubDialogChanged(IDialogProvider dialog) => DialogChanged?.Invoke(dialog);
}
