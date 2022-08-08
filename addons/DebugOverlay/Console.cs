#if DEBUG
namespace Additions.Debugging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[LogName("Debug")]
public class Console : MarginContainer
{
    [Export] public bool removeCommonStuff;
    [Export] public bool showTime = true;
    public List<string> commandHistory = new() { "" };
    public int CurrentHistoryIndex
    {
        get => currentHistoryIndex;
        set
        {
            currentHistoryIndex = value.Clamp(0, commandHistory.Count - 1);
            CommandLine.Text = commandHistory[currentHistoryIndex];
            CommandLine.CaretPosition = CommandLine.Text.Length;
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

    private CommandHandling storerForCommands;
    public CommandHandling Commands => this.LazyGetNode(ref storerForCommands, "Commands");

    #endregion
    #region HideCommonBtn Reference

    private Button storerForHideCommonBtn;
    public Button HideCommonBtn => this.LazyGetNode(ref storerForHideCommonBtn, _HideCommonBtn);
    [Export] private NodePath _HideCommonBtn = "HideCommonBtn";

    #endregion
    #region HideTimeBtn Reference

    private Button storerForHideTimeBtn;
    public Button HideTimeBtn => this.LazyGetNode(ref storerForHideTimeBtn, _HideTimeBtn);
    [Export] private NodePath _HideTimeBtn = "HideTimeBtn";

    #endregion

    public override void _Input(InputEvent @event)
    {
        if (CommandLine.HasFocus() && @event.IsPressed() && @event is InputEventKey keyInput)
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
        HideCommonBtn.SetPressedNoSignal(removeCommonStuff);
        HideTimeBtn.SetPressedNoSignal(!showTime);

        CurrentHistoryIndex = 0;
        CommandLine.Clear();
        CommandLine.CallDeferred("grab_focus");
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
    private void OnHideTimeToggled(bool toggled)
    {
        showTime = !toggled;
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
    private void LineEdited(string text)
    {
        commandHistory[0] = text;
    }

    [TroughtSignal]
    private void OnMetaClicked(string meta)
    {
        if (isReading)
            CommandLine.EmitSignal("text_entered", meta);

        OS.ShellOpen(meta);
    }

    public void RefreshOutput()
    {
        Output.Clear();

        StringBuilder sb = new();

        foreach (string item in DebugOverlay.outputLines)
        {
            string line = item;

            if (line.Length is 0)
            {
                sb.AppendLine();
                continue;
            }

            if (line.Contains("#TIME:"))
            {
                int timeStart = line.IndexOf("#TIME:");
                int timeLenght = (timeStart is -1) ? -1 : (line.IndexOf("#", timeStart + 1) - timeStart + 1);

                if (timeLenght is not -1)
                {
                    if (showTime) line = line.Replace(line.Substring(timeStart, timeLenght), $"[{line.Substring(timeStart + 6, timeLenght - 7)}] ");
                    else line = line.Remove(timeStart, timeLenght);
                }
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
        if (isReading) return;

        isReading = false;
        readCancellation.Cancel();
        DebugOverlay.AddOutputLine("Canceled", true);
    }

    internal async Task<string> ReadLine(string message, bool allowInvalidInput = false, params string[] options)
    {
        readCancellation = new();
        CancellationToken token = readCancellation.Token;

        DebugOverlay.AddOutputLine($"{message}\n{(allowInvalidInput ? "Reading input" : "")}{DebugOverlay.ColorizeText((string.Join("[/url]|[url]", options).Insert(0, "[url]") + "[/url]"), Colors.Wheat)}", true);
        RefreshOutput();

        isReading = true;

        string input = (await ToSignal(CommandLine, "text_entered"))[0] as string;

        if (token.IsCancellationRequested) return "";

        if (!allowInvalidInput && !options.Contains(input))
        {
            if (input is "" or " ") return "";

            isReading = false;
            DebugOverlay.AddOutputLine(DebugOverlay.ColorizeText("Invalid input", Colors.Gray), true);
            CallDeferred(nameof(RefreshOutput));
            return "";
        }

        isReading = false;
        CallDeferred(nameof(RefreshOutput));
        return input;
    }
}
#endif