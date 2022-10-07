namespace Diadot.Actions;

using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public abstract class DialogAction : Node
{
    [Export] public string command = "";

    public abstract Task Execute();
}