using Godot;
using System.Collections.Generic;
using Additions;

[Tool]
public class Foldout : Container
{
    [Signal] public delegate void Collapsed();
    [Signal] public delegate void Expanded();

    private bool collapsed = true;

    #region ContentContainer Reference

    #endregion

    [Export]
    public bool IsCollapsed
    {
        get => collapsed;
        set
        {
            collapsed = value;

            if (!IsInsideTree()) return;

            Control toHide = GetNode<Control>("HBoxContainer");

            if (value) toHide.Hide();
            else toHide.Show();
        }
    }

    public override void _Ready()
    {
        // Call setters
        IsCollapsed = IsCollapsed;
    }

    [TroughtSignal]
    private void OnButtonPressed()
    {
        IsCollapsed = !IsCollapsed;
    }
}
