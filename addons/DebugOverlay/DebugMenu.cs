#if DEBUG
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Godot;

namespace Additions.Debugging
{
    public class DebugMenu : Control
    {
        [Export] private NodePath _ExitButton, _PrintCommonLogsToggle, _NoUniqueNamesToggle, _ShowFpsToggle;

        [Export(PropertyHint.GlobalDir)] private string screenshotDir;
        [Export] private bool dateInScreenShot = true;

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
            printCommonLogsToggle.SetBlockSignals(true);
            noUniqueNamesToggle.SetBlockSignals(true);
            showFpsToggle.SetBlockSignals(true);

            printCommonLogsToggle.Pressed = !Overlay.turnOffCommonLogs;
            noUniqueNamesToggle.Pressed = Overlay.removeNameUniqeness;
            showFpsToggle.Pressed = Overlay.IsShowingFps;

            printCommonLogsToggle.SetBlockSignals(false);
            noUniqueNamesToggle.SetBlockSignals(false);
            showFpsToggle.SetBlockSignals(false);

            GetNode<Control>(_ExitButton).GrabFocus();
            EmitSignal(nameof(StateLoaded));
        }

        [TroughtSignal]
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

        [TroughtSignal]
        public async void TakeScreenshot(bool noCanvasLayers)
        {
            const float UiHideTime = 0.2f;

            Dictionary<Control, bool> prevVisibiliy = new();
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

            image.SavePng(fileDir);
            Debug.Log(this, $"Screenshot saved to {fileDir}");

            await new TimeAwaiter(this, UiHideTime);

            ResetCanvasLayerChildsVisibility(prevVisibiliy);

            void HideDebug()
            {
                foreach (Control child in Overlay.GetChildren<Control>())
                {
                    prevVisibiliy.Add(child, child.Visible);
                    child.Hide();
                }
            }

            void HideAllCanvasLayersChilds(ref Dictionary<Control, bool> prevVisibiliy)
            {
                GD.Print("HIDING");
                List<CanvasLayer> canvasLayers = new();
                GetTree().Root.GetAllChildren(ref canvasLayers);

                GD.Print(canvasLayers.Count);

                foreach (CanvasLayer canvasLayer in canvasLayers)
                {
                    if (canvasLayer == Overlay) continue;

                    foreach (Control child in canvasLayer.GetChildren<Control>())
                    {
                        prevVisibiliy.Add(child, child.Visible);
                        child.Hide();
                    }
                }
            }
            void ResetCanvasLayerChildsVisibility(Dictionary<Control, bool> prevVisibiliy)
            {
                foreach (KeyValuePair<Control, bool> item in prevVisibiliy)
                {
                    item.Key.Visible = item.Value;
                }
            }
        }

        [TroughtSignal]
        public void Quit()
        {
            GetTree().Quit();
        }

        [TroughtSignal]
        public void Exit()
        {
            Overlay.HideMenu();
        }

        [TroughtSignal]
        public void TogglePrintCommonLogs(bool toggled)
        {
            Overlay.turnOffCommonLogs = !toggled;
        }

        [TroughtSignal]
        public void ToggleNoUniqueNames(bool toggled)
        {
            Overlay.removeNameUniqeness = toggled;
        }

        [TroughtSignal]
        private void ToggleShowFps(bool toggled)
        {
            Overlay.IsShowingFps = toggled;
        }

        [TroughtSignal]
        private void PrintStrays()
        {
            PrintStrayNodes();
            Debug.LogU(this, "Printed stray nodes");
        }
    }
}
#endif