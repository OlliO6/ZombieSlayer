#if DEBUG
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Additions.Debugging
{
    public class Console : MarginContainer
    {
        public bool removeCommonStuff;
        public List<string> commandHistory = new() { "" };
        public int CurrentHistoryIndex
        {
            get => currentHistoryIndex;
            set
            {
                currentHistoryIndex = value.Clamp(0, commandHistory.Count - 1);
                CommandLine.Text = commandHistory[currentHistoryIndex];
            }
        }

        public bool isReading;
        private CancellationTokenSource readCancellation;
        private int currentHistoryIndex = 0;

        #region Output Reference

        private RichTextLabel storerForoutput;
        public RichTextLabel Output => this.LazyGetNode(ref storerForoutput, _output);
        [Export] private NodePath _output = "output";

        #endregion
        #region CommandLine Reference

        private LineEdit storerForCommandLine;
        public LineEdit CommandLine => this.LazyGetNode(ref storerForCommandLine, _CommandLine);
        [Export] private NodePath _CommandLine = "CommandLine";

        #endregion
        #region Commands Reference

        private Commands storerForCommands;
        public Commands Commands => this.LazyGetNode(ref storerForCommands, "Commands");

        #endregion

        public override void _Input(InputEvent @event)
        {
            if (@event.IsPressed() && @event is InputEventKey keyInput)
            {
                if (keyInput.Scancode is (uint)KeyList.Up)
                {
                    CurrentHistoryIndex++;
                    GetTree().SetInputAsHandled();
                    return;
                }
                if (keyInput.Scancode is (uint)KeyList.Down)
                {
                    CurrentHistoryIndex--;
                    GetTree().SetInputAsHandled();
                    return;
                }
            }
        }

        public void Setup()
        {
            CurrentHistoryIndex = 0;
            CommandLine.Clear();
            CommandLine.GrabClickFocus();
            RefreshOutput();
        }

        [TroughtSignal]
        private void OnBackPressed() => (Owner as DebugOverlay)?.CloseConsole();

        [TroughtSignal]
        private void OnHideCommonStuffToggled(bool toggled)
        {
            removeCommonStuff = toggled;
            RefreshOutput();
        }

        [TroughtSignal]
        private void OnCommandEntered(string command)
        {
            CommandLine.Clear();

            if (isReading) return;

            Commands.Execute(command);
            RefreshOutput();

            commandHistory.Insert(1, command);
            CurrentHistoryIndex = 0;
        }

        [TroughtSignal]
        private void OnMetaClicked(string meta)
        {
            OS.ShellOpen(meta);
        }

        public void RefreshOutput()
        {
            Output.Clear();

            StringBuilder sb = new();

            foreach (string line in DebugOverlay.outputLines)
            {
                if (line.Length is 0)
                {
                    sb.AppendLine();
                    continue;
                }

                if (line.BeginsWith("#"))
                {
                    int uniquenessLenght = line.IndexOf("#:");

                    if (uniquenessLenght is -1)
                    {
                        sb.AppendLine(line);
                        continue;
                    }

                    string uniqueness = line.Substring(1, uniquenessLenght - 1);

                    if (removeCommonStuff && uniqueness is "COMMON") continue;

                    sb.AppendLine(line.Substring(uniquenessLenght + 2));
                    continue;
                }
                sb.AppendLine(line);
            }

            Output.BbcodeText = sb.ToString();
        }

        public void ClearOutput()
        {
            DebugOverlay.outputLines = new();
            RefreshOutput();
        }

        internal void CancelReading()
        {
            isReading = false;
            readCancellation.Cancel();
        }

        internal async Task<string> ReadLine(string message, params string[] options)
        {
            readCancellation = new();
            CancellationToken token = readCancellation.Token;

            DebugOverlay.AddOutputLine($"{message}\n{DebugOverlay.ColorizeText(string.Join(" ", options), Colors.Wheat)}", true);
            RefreshOutput();

            isReading = true;

            SignalAwaiter signalAwaiter = ToSignal(CommandLine, "text_entered");
            await signalAwaiter;

            if (token.IsCancellationRequested) return "";

            string input = (string)signalAwaiter.GetResult()[0];

            if (!options.Contains(input))
            {
                if (input is "" or " ") return "";

                DebugOverlay.AddOutputLine(DebugOverlay.ColorizeText("Invalid input", Colors.Gray), true);
                RefreshOutput();
                return "";
            }

            return input;
        }
    }
}

#endif