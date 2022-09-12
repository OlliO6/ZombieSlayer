namespace Diadot;

using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class DialogPlayer : CanvasLayer
{
    public Label nameLabel;
    public AnimatedRichTextLabel textLabel;

    public IDialogProvider currentDialog;

    public override void _Ready()
    {
        nameLabel = GetNode<Label>("%NameLabel");
        textLabel = GetNode<AnimatedRichTextLabel>("%TextLabel");

        textLabel.Connect(nameof(AnimatedRichTextLabel.Advanced), this, nameof(OnTextAdvanced));
        textLabel.Connect(nameof(AnimatedRichTextLabel.Finished), this, nameof(OnTextFinished));
        Hide();
    }

    private void OnTextFinished()
    {
        currentDialog.OnTextFinished();
    }
    private void OnTextAdvanced()
    {
        currentDialog.OnTextAdvanced();
    }

    public Error Play(string dialogName)
    {
        currentDialog = GetNodeOrNull<IDialogProvider>(dialogName);

        if (currentDialog is null)
            return Error.DoesNotExist;

        Show();

        textLabel.Play("[expressions]" + currentDialog.Text);

        currentDialog.Ended += (_) =>
        {
            currentDialog = null;
            Hide();
        };

        Debug.Log(this, $"{dialogName} started");
        return Error.Ok;
    }
}
