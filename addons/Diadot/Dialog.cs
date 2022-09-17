namespace Diadot;

using System;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Godot;

public class Dialog : Node, IDialogProvider
{
    [Export(PropertyHint.MultilineText)] private string text = "";
    [Export] private string character = "";
    [Export] private bool giveOptions;
    [Export] private NodePath[] callableActions = new NodePath[0];
    [Export] private NodePath[] onStartActions = new NodePath[0];
    [Export] private NodePath[] onFinishActions = new NodePath[0];

    public bool waitingForInput;

    public event Action<IDialogProvider, NodePath> Ended;
    public event Action<IDialogProvider> DialogChanged;

    private string selectedOption;

    public string Text { get => text; set => text = value; }

    public string Character { get => character; set => character = value; }

    public void OnTextAdvanced() { }

    public void OnTextFinished()
    {
        waitingForInput = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!giveOptions && waitingForInput && @event.IsActionPressed(ProjectSettingsControl.SkipInput))
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

        foreach (NodePath path in onStartActions)
        {
            Action action = GetNodeOrNull<Action>(path);
            if (action is not null) action.Execute();
        }
    }

    public void Finish()
    {
        waitingForInput = false;

        foreach (NodePath path in onFinishActions)
        {
            Action action = GetNodeOrNull<Action>(path);
            if (action is not null) action.Execute();
        }

        IDialogProvider dialog = GetDialog();

        if (dialog is null)
        {
            Ended?.Invoke(this, Name);
            return;
        }

        dialog.DialogChanged += OnSubDialogChanged;
        dialog.Ended += OnSubDialogEnded;
        DialogChanged?.Invoke(dialog);

        IDialogProvider GetDialog()
        {
            if (giveOptions)
                return GetNodeOrNull<IDialogProvider>(selectedOption);

            if (GetChildCount() > 0)
                return GetChild<IDialogProvider>(0);

            return null;
        }
    }

    private void OnSubDialogEnded(IDialogProvider sender, NodePath dialog)
    {
        sender.Ended -= OnSubDialogEnded;
        sender.DialogChanged -= OnSubDialogChanged;
        Ended?.Invoke(this, dialog + Name);
    }

    private void OnSubDialogChanged(IDialogProvider dialog) => DialogChanged?.Invoke(dialog);

    public string[] GetOptions()
    {
        if (giveOptions)
            return this.GetChildren<IDialogProvider>()
                                .Select<IDialogProvider, string>((IDialogProvider dialogProvider) => dialogProvider.Name).ToArray();

        return null;
    }

    public void ProcessOptionPress(string option)
    {
        if (!waitingForInput)
            return;

        selectedOption = option;
        Finish();
    }

    public void ProcessUnhandeledExpression(string expression)
    {
        foreach (NodePath path in callableActions)
        {
            Action action = GetNodeOrNull<Action>(path);
            if (action is null) continue;

            if (action.command == expression)
            {
                action.Execute();
                return;
            }
        }
    }
}
