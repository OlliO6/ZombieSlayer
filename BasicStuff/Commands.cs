#if DEBUG
using Additions;
using Additions.Debugging;
using Godot;

class GameCommands : Object
{
    void GivePlayerCoins()
    {
    }
}

[DefaultColor(nameof(Colors.LightSteelBlue), nameof(Colors.SteelBlue))]
public class OptionsCommands : CommandCollection
{
    public override string CollectionName => "Options";
    public override string Description => "Commands for the options manager.";

    [Command("saveoptions", "Saves the current options.")]
    void Save()
    {
        OptionsManager.SaveOptions();
        DebugOverlay.RefreshOutput();
    }
    [Command("resetoptions", "Resets current options to default.")]
    void Reset()
    {
        OptionsManager.ResetOptions();
        AddOutputLine("Set options to default");
        RefreshOutput();
    }
}








#endif