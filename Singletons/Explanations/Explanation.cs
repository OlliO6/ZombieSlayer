using System;
using System.Collections.Generic;
using Additions;
using Godot;

public abstract class Explanation : Control
{
    [Signal] public delegate void Started();
    [Signal] public delegate void Ended();

    public void Beginn()
    {
        Start();
        Debug.Log(this, "Explanation Started");
        EmitSignal(nameof(Started));
    }

    public void Finish()
    {
        End();
        Debug.Log(this, "Explanation Ended");
        EmitSignal(nameof(Ended));
    }

    protected abstract void Start();
    protected abstract void End();
}
