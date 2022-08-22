#if DEBUG
using Additions;
using Additions.Debugging;
using Godot;

[DefaultColor(nameof(Colors.LightBlue), nameof(Colors.DeepSkyBlue))]
public class GameCommands : CommandCollection
{
    public override string CollectionName => "Game";
    public override string Description => "Control over the whole game.";

    #region Player  

    [Command("giveCoins", "Give coins to the player.")]
    void GiveCoins(int amount)
    {
        if (!PlayerAvailable()) return;

        CurrentPlayer.Coins += amount;
        AddOutputLine(ColorizeText($"Gave {amount} coins to player", Colors.Gold));
    }

    [Command("heal", "Heal the player.")]
    void HealPlayer()
    {
        if (!PlayerAvailable()) return;

        CurrentPlayer.CurrentHealth = CurrentPlayer.MaxHealth;
        AddOutputLine(ColorizeText("Restored player health", Colors.Gold));
    }

    [Command("giveUpgrade", "Give an upgrade to the player.")]
    void GiveUpgrade(string what)
    {
        if (!PlayerAvailable()) return;

        if (!what.EndsWith("Upgrade")) what = what + "Upgrade";

        System.Type type = System.Type.GetType(what, false, true);
        if (type is null)
        {
            AddOutputLine(ColorizeText("Specified upgrade type is not valid", Colors.OrangeRed));
            return;
        }

        CurrentPlayer.AddUpgrade(System.Activator.CreateInstance(type) as Upgrade);
        AddOutputLine(ColorizeText($"{type.Name} added to player", Colors.DodgerBlue));
    }

    [Command("removeUpgrade", "Removes an upgrade from the player.")]
    void RemoveUpgrade(string what)
    {
        if (!PlayerAvailable()) return;

        if (!what.EndsWith("Upgrade")) what = what + "Upgrade";

        System.Type type = System.Type.GetType(what, false, true);
        if (type is null)
        {
            AddOutputLine(ColorizeText("Specified upgrade type is not valid", Colors.OrangeRed));
            return;
        }

        foreach (var item in CurrentPlayer.GetUpgrades())
        {
            if (item.GetType() == type)
            {
                item.QueueFree();
                AddOutputLine(ColorizeText($"Removed {type.Name} from the player", Colors.DodgerBlue));
                return;
            }
        }

        AddOutputLine(ColorizeText($"Player don't has an {type.Name}", Colors.Orange));
    }

    [Command("lvlup", "Increases player lavel.")]
    void LvlUp()
    {
        if (!PlayerAvailable()) return;

        CurrentPlayer.Leveling.CurrentXp += CurrentPlayer.Leveling.CurrentLevelNode.xpToLevelUp;
    }

    private bool PlayerAvailable()
    {
        if (IsInstanceValid(Player.currentPlayer)) return true;

        AddOutputLine(ColorizeText("No player available", Colors.OrangeRed));
        return false;
    }
    private Player CurrentPlayer => Player.currentPlayer;

    #endregion Player 
}

[DefaultColor(nameof(Colors.LightSteelBlue), nameof(Colors.SteelBlue))]
public class OptionsCommands : CommandCollection
{
    public override string CollectionName => "Options";
    public override string Description => "Commands for the options manager.";

    [Command("saveOptions", "Saves the current options.")]
    void Save()
    {
        OptionsManager.SaveOptions();
        DebugOverlay.RefreshOutput();
    }

    [Command("resetOptions", "Resets current options to default.")]
    void Reset()
    {
        OptionsManager.ResetOptions();
        AddOutputLine("Set options to default");
        RefreshOutput();
    }
}








#endif