#if DEBUG
using System.Text;
using Godot;

namespace Additions.Debugging
{
    public class Console : MarginContainer
    {
        public bool removeCommonStuff;


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

        [TroughtSignal]
        private void OnHideCommonStuffToggled(bool toggled)
        {
            removeCommonStuff = toggled;
            RefreshOutput();
        }

        public void Setup()
        {
            CommandLine.Clear();
            CommandLine.GrabClickFocus();
            RefreshOutput();
        }

        [TroughtSignal]
        private void OnBackPressed() => (Owner as DebugOverlay)?.CloseConsole();

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
    }
}

#endif