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
    [Export] public string explanation = "";
    [Export] public bool noMenu, dontHeal;

    private static PackedScene lvlUpDispScene = GD.Load<PackedScene>("res://UI/Leveling/LevelUpDisplay.tscn");

    public void ReachLevel()
    {
        Reached();
        Debug.LogU(this, "Reached");

        IEnumerable<LevelBuff> buffs = this.GetChildren<LevelBuff>();

        if (!dontHeal)
            Player.currentPlayer?.Heal();

        EmitSignal(nameof(LevelReached));

        if (!noMenu)
        {
            LevelUpDisplay menu;
            menu = MakeMenu();
            ToSignal(menu, nameof(LevelUpDisplay.CloseBegan)).OnCompleted(() => ApllyBuffs(buffs));
            ToSignal(menu, nameof(LevelUpDisplay.Closed)).OnCompleted(() =>
            {
                if (explanation is not "")
                    ExplanationsManager.StartExplanation(explanation);
                menu.QueueFree();
            });
            return;
        }

        ApllyBuffs(buffs);
        if (explanation is not "")
            ExplanationsManager.StartExplanation(explanation);

        static void ApllyBuffs(IEnumerable<LevelBuff> buffs)
        {
            foreach (LevelBuff buff in buffs)
            {
                buff.Apply();
                buff.EmitSignal(nameof(LevelBuff.Applied));
            }
        }

        LevelUpDisplay MakeMenu()
        {
            LevelUpDisplay menu = lvlUpDispScene.Instance<LevelUpDisplay>();
            AddChild(menu);

            menu.SetTitle($"Reached {Name}");

            foreach (LevelBuff buff in buffs)
            {
                menu.AddBuffText(buff);
            }

            menu.Open();
            return menu;
        }
    }

    protected virtual void Reached() { }
}
