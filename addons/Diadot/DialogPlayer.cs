namespace Diadot;

using System;
using System.Collections.Generic;
using Additions;
using Godot;

public class DialogPlayer : CanvasLayer
{
    [Signal] public delegate void DialogStarted(string dialog);
    [Signal] public delegate void DialogFinished(string dialog, NodePath exitPath);

    public Label nameLabel;
    public HBoxContainer optionsContainer;
    public AnimatedRichTextLabel textLabel;

    public IDialogProvider currentDialog;

    public override void _Ready()
    {
        nameLabel = GetNode<Label>("%NameLabel");
        optionsContainer = GetNode<HBoxContainer>("%OptionButtons");
        textLabel = GetNode<AnimatedRichTextLabel>("%TextLabel");

        textLabel.Connect(nameof(AnimatedRichTextLabel.Advanced), this, nameof(OnTextAdvanced));
        textLabel.Connect(nameof(AnimatedRichTextLabel.Finished), this, nameof(OnTextFinished));
        Hide();
    }

    private void OnTextFinished() => currentDialog?.OnTextFinished();
    private void OnTextAdvanced() => currentDialog?.OnTextAdvanced();
    private void OnOptionButtonPressed(string option) => currentDialog?.ProcessOptionPress(option);

    public Error Play(string dialogName)
    {
        IDialogProvider dialog = GetNodeOrNull<IDialogProvider>(dialogName);

        if (dialog is null)
            return Error.DoesNotExist;

        Play(dialog);

        return Error.Ok;
    }

    public void Play(IDialogProvider dialog)
    {
        Show();

        ChangeDialog(dialog);
        dialog.Ended += OnDialogEnded;
        dialog.DialogChanged += ChangeDialog;

        Debug.Log(this, $"{dialog.Name} started");
    }

    private void ChangeDialog(IDialogProvider dialog)
    {
        currentDialog = dialog;

        ClearOptions();
        string[] options = dialog.GetOptions();
        if (options is not null)
            InitOptions(options);

        nameLabel.Text = dialog.Character;
        textLabel.Play("[expressions]" + dialog.Text);
        dialog.OnStarted();
        EmitSignal(nameof(DialogStarted), dialog.Name);

        void InitOptions(string[] options)
        {
            foreach (var option in options)
            {
                Button button = new Button()
                {
                    EnabledFocusMode = Control.FocusModeEnum.None,
                    Flat = true,
                    Text = option
                };
                optionsContainer.AddChild(button);
                button.Connect("pressed", this, nameof(OnOptionButtonPressed), new() { option });
                button.Hide();

                textLabel.Connect(nameof(AnimatedRichTextLabel.Finished), button, "show");
            }
        }
    }

    private void OnDialogEnded(IDialogProvider sender, NodePath dialog)
    {
        currentDialog = null;
        sender.Ended -= OnDialogEnded;
        sender.DialogChanged -= ChangeDialog;
        EmitSignal(nameof(DialogFinished), sender.Name, dialog);
        ClearOptions();
        Hide();

        Debug.Log(this, $"{sender.Name} ended");
    }

    private void ClearOptions()
    {
        for (int i = 0; i < optionsContainer.GetChildCount(); i++)
        {
            optionsContainer.GetChild(i).QueueFree();
        }
    }
}
