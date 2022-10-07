namespace Diadot.Actions;

using System.Collections.Generic;
using System.Threading.Tasks;
using Additions;
using Godot;

public class ToSignal : DialogAction
{
    [Export] public NodePath path;
    [Export] public Godot.Object obj;
    [Export] public bool usePath = true;
    [Export] public string signal = "";

    public override async Task Execute()
    {
        await ToSignal(usePath ? GetNode(path) : obj, signal);
    }
}