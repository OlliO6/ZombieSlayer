using Godot;
using System;

public class UIManager : CanvasLayer
{
    public static UIManager Instance { get; private set; }

    private Container descriptionContainer;
    private Label description;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        descriptionContainer = GetNode<Container>("%DescriptionContainer");
        description = GetNode<Label>("%Description");
    }

    public static void ShowDescription(string text, Control control) => Instance._ShowDescription(text, control);

    public static void ShowDescription(string text, Vector2 pos) => Instance._ShowDescription(text, pos);

    public static void HideDescription(string text) => Instance._HideDescription(text);

    public static void HideDescription() => Instance._HideDescription();

    public void _ShowDescription(string text, Control control) => _ShowDescription(text, control.GetGlobalRect().GetCenter());

    public void _ShowDescription(string text, Vector2 pos)
    {
        if (text is "" or null)
            return;

        descriptionContainer.Show();
        descriptionContainer.RectGlobalPosition = pos;
        description.Text = text;
    }

    public void _HideDescription(string text)
    {
        if (description.Text == text)
            descriptionContainer.Hide();
    }

    public void _HideDescription() => descriptionContainer.Hide();
}
