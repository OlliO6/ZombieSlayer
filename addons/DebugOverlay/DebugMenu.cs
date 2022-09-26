#if DEBUG
namespace Additions.Debugging;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Godot;

[LogName("Debug")]
public class DebugMenu : Control
{
    [Export] private NodePath _ExitButton, _PrintCommonLogsToggle, _NoUniqueNamesToggle, _ShowFpsToggle;

    [Export(PropertyHint.GlobalDir)] internal string screenshotDir;
    [Export] internal bool dateInScreenShot = true;

    [Signal] public delegate void StateLoaded();

    public CancellationTokenSource stepCancellation = new();
    private Button printCommonLogsToggle, noUniqueNamesToggle, showFpsToggle;
    DebugOverlay Overlay => Owner.LazyObjectCast(ref storerForOverlay);
    DebugOverlay storerForOverlay;

    public override void _Ready()
    {
        printCommonLogsToggle = GetNode<Button>(_PrintCommonLogsToggle);
        noUniqueNamesToggle = GetNode<Button>(_NoUniqueNamesToggle);
        showFpsToggle = GetNode<Button>(_ShowFpsToggle);

        RectSize = RectMinSize;
    }

    public void LoadState()
    {
        printCommonLogsToggle.SetPressedNoSignal(!Overlay.turnOffCommonLogs);
        noUniqueNamesToggle.SetPressedNoSignal(Overlay.removeNameUniqeness);
        showFpsToggle.SetPressedNoSignal(Overlay.IsShowingFps);

        GetNode<Control>(_ExitButton).GrabFocus();
        EmitSignal(nameof(StateLoaded));
    }

    [TroughtEditor]
    public async void FrameStep(int times, bool physics)
    {
        await ToSignal(GetTree(), physics ? "physics_frame" : "idle_frame");
        stepCancellation.Cancel();
        stepCancellation = new();
        var token = stepCancellation.Token;

        GetTree().Paused = false;

        for (int i = 0; i < times; i++) await ToSignal(GetTree(), physics ? "physics_frame" : "idle_frame");

        if (token.IsCancellationRequested) return;
        GetTree().Paused = true;
    }

    [TroughtEditor]
    public async void TakeScreenshot(bool noCanvasLayers)
    {
        const float UiHideTime = 0.2f;

        Dictionary<CanvasLayer, bool> prevVisibiliy = new();
        if (noCanvasLayers) HideAllCanvasLayersChilds(ref prevVisibiliy);
        HideDebug();

        await ToSignal(GetTree(), "idle_frame");
        await ToSignal(GetTree(), "idle_frame");

        Image image = GetTree().Root.GetTexture().GetData();
        image.FlipY();

        Directory dir = new();

        if (!dir.DirExists(screenshotDir))
        {
            dir.MakeDirRecursive(screenshotDir);
        }

        string fileName = "Screenshot";

        if (dateInScreenShot)
        {
            var dateDict = OS.GetDate();
            StringBuilder date = new();

            date.Append(dateDict["day"]).Append(".")
                    .Append(dateDict["month"]).Append(".")
                    .Append(dateDict["year"]);

            fileName += $"({date.ToString()})";
        }
        fileName += ".png";

        #region Make file name unique 

        dir.Open(screenshotDir);
        dir.ListDirBegin();
        List<string> fileNames = new();
        string currentFile = dir.GetNext();

        while (currentFile is not "")
        {
            if (!dir.CurrentIsDir())
                fileNames.Add(currentFile);

            currentFile = dir.GetNext();
        }

        int number = 0;

        while (fileNames.Contains(NumberisedFileName(fileName, number)))
        {
            number++;
        }

        fileName = NumberisedFileName(fileName, number);

        string NumberisedFileName(string fileName, int number)
        {
            if (number <= 0) return fileName;
            return $"{fileName.RStrip(".png")}_{number}.png";
        }

        #endregion Make file name unique

        string fileDir = screenshotDir.PlusFile(fileName);

        Debug.LogU(this, "Saved screenshot");

        await new TimeAwaiter(this, UiHideTime);

        image.SavePng(fileDir);

        ResetCanvasLayerChildsVisibility(prevVisibiliy);

        void HideDebug()
        {
            prevVisibiliy.Add(Overlay, Overlay.Visible);
            Overlay.Hide();
        }

        void HideAllCanvasLayersChilds(ref Dictionary<CanvasLayer, bool> prevVisibiliy)
        {
            List<CanvasLayer> canvasLayers = new();
            GetTree().Root.GetAllChildren(ref canvasLayers);

            foreach (CanvasLayer canvasLayer in canvasLayers)
            {
                if (canvasLayer == Overlay) continue;

                prevVisibiliy.Add(canvasLayer, canvasLayer.Visible);
                canvasLayer.Hide();
            }
        }
        void ResetCanvasLayerChildsVisibility(Dictionary<CanvasLayer, bool> prevVisibiliy)
        {
            foreach (KeyValuePair<CanvasLayer, bool> item in prevVisibiliy)
            {
                item.Key.Visible = item.Value;
            }
        }
    }

    [TroughtEditor]
    public void Quit() => GetTree().Quit();

    [TroughtEditor]
    public void Crash() => OS.Crash("Crashed from debug overlay");

    [TroughtEditor]
    public void Exit() => Overlay.HideMenu();

    [TroughtEditor]
    private void OpenConsole() => Overlay.OpenConsole();

    [TroughtEditor]
    public void TogglePrintCommonLogs(bool toggled) => Overlay.turnOffCommonLogs = !toggled;

    [TroughtEditor]
    public void ToggleNoUniqueNames(bool toggled) => Overlay.removeNameUniqeness = toggled;

    [TroughtEditor]
    private void ToggleShowFps(bool toggled) => Overlay.IsShowingFps = toggled;

    [TroughtEditor]
    private void PrintStrays()
    {
        PrintStrayNodes();
        Debug.LogU(this, "Printed stray nodes");
    }
}
#endif