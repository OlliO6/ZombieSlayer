using Additions;
using Godot;

[Additions.Debugging.DefaultColor(nameof(Colors.Aqua))]
public abstract class Upgrade : Node
{
    public static string GetDescription() => "";

    public override void _EnterTree()
    {
        if (Player.currentPlayer is not null)
        {
            AddBuff(Player.currentPlayer);
            Debug.LogU(this, "Buff added");
        }
    }

    public override void _ExitTree()
    {
        if (Player.currentPlayer is not null)
        {
            RemoveBuff(Player.currentPlayer);
            Debug.LogU(this, "Buff removed");
        }
    }

    public abstract void AddBuff(Player player);
    public abstract void RemoveBuff(Player player);
}
