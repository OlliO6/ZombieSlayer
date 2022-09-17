namespace Diadot;
using System.Collections.Generic;
using Additions;
using Godot;

public abstract class Action : Node
{
    [Export] public string command = "";

    public abstract void Execute();
}
