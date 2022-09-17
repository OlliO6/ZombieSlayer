namespace Diadot;
using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class RedirectDialog : Node, IDialogProvider
{
    [Export] private NodePath to;

    public bool waitingForInput;
    public event Action<IDialogProvider, NodePath> Ended;
    public event Action<IDialogProvider> DialogChanged;

    public string Text => string.Empty;
    public string Character => string.Empty;

    public void Finish()
    {
        IDialogProvider dialog = GetNode<IDialogProvider>(to);
        if (dialog is null)
        {
            Ended?.Invoke(this, Name);
            return;
        }

        DialogChanged?.Invoke(dialog);
        dialog.DialogChanged += OnSubDialogChanged;
        dialog.Ended += OnSubDialogEnded;
    }

    private void OnSubDialogEnded(IDialogProvider sender, NodePath dialog)
    {
        sender.Ended -= OnSubDialogEnded;
        sender.DialogChanged -= OnSubDialogChanged;
        Ended?.Invoke(this, dialog + Name);
    }

    public void OnStarted() => Finish();
    private void OnSubDialogChanged(IDialogProvider dialog) => DialogChanged?.Invoke(dialog);
    public void OnTextAdvanced() { }
    public void OnTextFinished() { }
    public string[] GetOptions() => null;
    public void ProcessOptionPress(string option) { }
}
