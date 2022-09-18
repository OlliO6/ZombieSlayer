namespace Diadot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public abstract class Action : Node
{
    [Export] public string command = "";

    public abstract Task Execute();
}
