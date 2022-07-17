using Godot;
using System;

public class DamageIndikator : Label
{
    public override void _Ready()
    {
        Theme = GD.Load<Theme>("res://UI/Theme/Theme.tres");
    }
}
