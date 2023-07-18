#if DEBUG
namespace Additions.Debugging;
using Godot;

[DefaultColor(nameof(Colors.Wheat))]
public class GenericCommands : CommandCollection
{
    public override string CollectionName => "Generic";
    public override string Description => "Commands that are game independent.";

    [Command("print", "Print something to the console.")]
    private void PrintLine(string what) => DebugOverlay.AddOutputLine(what, true);

    [Command("quit", "Quits the game.")]
    private void Quit() => sceneTree.Quit();

    [Command("exit", "Exit the debug overlay.")]
    private void Exit()
    {
        DebugOverlay.instance.CloseConsole();
        DebugOverlay.instance.HideMenu();
    }

    [Command("clear", "Clears the console.")]
    private void Clear() => console.ClearOutput();

    [Command("hideCommon", "Sets if common logs should be hidden.")]
    private void HideCommonLogs(bool hiding) => console.HideCommonBtn.Pressed = hiding;

    [Command("hideTime", "Sets if the time of logs should be shown.")]
    private void HideLogTiem(bool hiding) => console.HideTimeBtn.Pressed = hiding;

    [Command("screenshot", "Takes a screenshot with or without the games UI.")]
    private async void Screenshot()
    {
        string input = await console.ReadLine("Hide game UI", false, "yes", "no");

        if (input is "") return;

        DebugOverlay.instance.Menu.TakeScreenshot(input is "yes");
    }

    [Command("screenshotDir", "Shows the folder where your screendhots are getting saved.")]
    private void ScreenshotDir()
    {
        DebugOverlay.AddOutputLine($"Your screenshot directory is here [url]{DebugOverlay.instance.Menu.screenshotDir}[/url]", true);
    }

    [Command("setScreenshotDir", "Set the directory where you want your screenshots to be saved.")]
    private async void SetScreenshotDir(string path)
    {
        Directory dir = new();

        if (!dir.DirExists(path))
        {
            string input = await console.ReadLine($"{DebugOverlay.ColorizeText($"directory {path} doesn't exists. Create it now?", Colors.Gray)}", false, "yes", "no");

            if (input is not "yes")
            {
                DebugOverlay.AddOutputLine("Set screenshot dir canceled", true);
                return;
            }
            Error error = dir.MakeDirRecursive(path);

            if (error is not Error.Ok)
            {
                DebugOverlay.AddOutputLine($"Couldn't create the directory {DebugOverlay.instance.Menu.screenshotDir}, Eroor: {error}", true);
                return;
            }

            DebugOverlay.AddOutputLine($"Succesfully created the screenshot directory", true);
        }

        if (path.StartsWith("res://") || path.StartsWith("user://")) path = ProjectSettings.GlobalizePath(path);

        DebugOverlay.instance.Menu.screenshotDir = path;
        DebugOverlay.AddOutputLine($"Succesfully set the screenshot directory to [url]{DebugOverlay.instance.Menu.screenshotDir}[/url]", true);
        DebugOverlay.instance.Save();
    }

    [Command("dateInScreenshot", "Control if the date will be shown in new screenshots.")]
    private void DateInScreenshot(bool show)
    {
        DebugOverlay.instance.Menu.dateInScreenShot = show;
        DebugOverlay.AddOutputLine($"The date will now be {(show ? "shown" : "hidden")} in screenshots", true);
    }
}
#endif