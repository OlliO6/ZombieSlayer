namespace Leveling;
using System.Collections.Generic;
using System.Linq;
using Additions;
using Buffs;
using Godot;

[Additions.Debugging.DefaultColor(nameof(Colors.Aquamarine))]
public class Level : Node
{
    [Signal] public delegate void LevelReached();

    [Export] public int xpToLevelUp = 100;
    [Export] private bool noMenu = false;

    private static PackedScene lvlUpDispScene = GD.Load<PackedScene>("res://UI/Leveling/LevelUpDisplay.tscn");

    public void ReachLevel()
    {
        Reached();
        EmitSignal(nameof(LevelReached));
        Debug.LogU(this, "Reached");

        IEnumerable<LevelBuff> buffs = this.GetChildren<LevelBuff>();

        ApllyBuffs(buffs);
        if (!noMenu) MakeMenu();

        static void ApllyBuffs(IEnumerable<LevelBuff> buffs)
        {
            foreach (LevelBuff buff in buffs)
                buff.Apply();
        }

        async void MakeMenu()
        {
            LevelUpDisplay menu = lvlUpDispScene.Instance<LevelUpDisplay>();
            AddChild(menu);

            await ToSignal(GetTree(), "idle_frame");

            menu.SetTitle($"Reached {Name}");

            foreach (LevelBuff buff in buffs)
            {
                menu.AddBuffText(buff);
            }

            menu.Open();
            ToSignal(menu, nameof(LevelUpDisplay.Closed)).OnCompleted(menu.QueueFree);
        }
    }

    protected virtual void Reached() { }
}
