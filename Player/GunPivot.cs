using Godot;
using System;

public class GunPivot : Node2D
{
    public override void _Process(float delta)
    {
        Vector2 mousePos = GetGlobalMousePosition();

        LookAt(mousePos);

        if (GlobalPosition.x > mousePos.x)
        {
            Scale = new Vector2(-1, 1);
            Rotate(Mathf.Deg2Rad(180));
        }
        else
        {
            Scale = new Vector2(1, 1);
        }
    }
}
