using System;
using Godot;

[Tool]
public class GradiantProgressBar : ProgressBar
{
    [Export] private Gradient gradient;
    [Export] private bool useFlatStyleBgColor = true;

    public override void _Ready()
    {
        Connect("value_changed", this, nameof(UpdateColor));
        UpdateColor();
    }

    private void UpdateColor(float value = 0)
    {
        if (useFlatStyleBgColor)
        {
            (GetStylebox("fg") as StyleBoxFlat).BgColor = gradient.Interpolate((float)(Value / MaxValue));
            return;
        }
    }
}
