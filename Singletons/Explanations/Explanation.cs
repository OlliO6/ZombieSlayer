namespace Explanations;
using System;
using System.Collections.Generic;
using Additions;
using Godot;

public abstract class Explanation : Control
{
    [Signal] public delegate void Started();
    [Signal] public delegate void Ended();

    public bool running;

    public void Beginn()
    {
        running = true;
        Start();
        Debug.Log(this, "Explanation Started");
        EmitSignal(nameof(Started));
    }

    public void Finish()
    {
        running = false;
        End();
        Debug.Log(this, "Explanation Ended");
        EmitSignal(nameof(Ended));
    }

    protected abstract void Start();
    protected abstract void End();
}
