#if DEBUG
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Additions.Debugging
{
    public class Commands : Node
    {
        public Dictionary<string, Command> availableCommands;
        public Dictionary<string, Godot.Object> availablePointers;

        private Console Console => DebugOverlay.instance.Console;

        static void Main()
        {
            new Console();
            System.Console.ReadLine();
        }

        public override void _Ready()
        {
            Assembly assembly = typeof(Commands).Assembly;

            foreach (TypeInfo type in assembly.DefinedTypes) //TODO attribute that makes new commands
            {
                // type.GetCustomAttribute<>() 
            }

            availableCommands = new()
            {
                {
                    "help",
                    new Command(this, nameof(Help), new ArgType[] { }, "Show available commands")
                },
                {
                    "print",
                    new Command(this, nameof(PrintLine), new ArgType[] { ArgType.String }, "Print something to the console")
                },
                {
                    "log",
                    new Command(this, nameof(PrintLine), new ArgType[] { ArgType.String }, "Same as print")
                },
                {
                    "screenshotdir",
                    new Command(this, nameof(ScreenshotDir), new ArgType[] { }, "Open the folder where your screenshot are saved")
                },
                {
                    "setscreenshotdir",
                    new Command(this, nameof(SetScreenshotDir), new ArgType[] { ArgType.String }, "Set the directory where you want your screenshots to be saved")
                },
                {
                    "dateinscreenshot",
                    new Command(this, nameof(DateInScreenshot), new ArgType[] { ArgType.Bool }, "Control if the date will be shown in new screenshots")
                }
            };

        }

        public void Execute(string command)
        {
            if (command is "") return;

            // Make string arg possible
            if (command.Contains('"'))
            {
                if (command.Count((char c) => c is '"') % 2 is not 0)
                {
                    DebugOverlay.AddOutputLine(DebugOverlay.ColorizeText("Failed executing because of unclosed string", Colors.Gray), true);
                    return;
                }

                string[] splitted = ($"%>space<%{command}%>space<%").Split('"');
                for (int i = 0; i < splitted.Length; i++)
                {
                    if (i % 2 is not 0)
                    {
                        splitted[i] = $"\"{splitted[i].Replace(" ", "%>space<%")}\"";
                    }
                }
                command = "";
                foreach (string item in splitted) command += item;
                command = command.Remove(0, 9).Remove(command.Length - 18);
            }

            command = command.StripEdges();

            string[] parts = command.Split(' ').Where((part) => part is not " " and not "").ToArray();

            if (parts.Length is 0) return;

            parts[0] = parts[0].ToLower();

            if (!availableCommands.ContainsKey(parts[0]))
            {
                DebugOverlay.AddOutputLine($"{DebugOverlay.ColorizeText("Command", Colors.Gray)} '{parts[0]}' {DebugOverlay.ColorizeText("is not available", Colors.Gray)}", true);
                return;
            }

            Command cm = availableCommands[parts[0]];

            string[] args = parts.Length > 0 ? parts.Skip(1).ToArray() : new string[0];

            if (!ArgsValid())
            {
                DebugOverlay.AddOutputLine($"{DebugOverlay.ColorizeText("[shake]Failed executing[/shake]", Colors.Gray)} {DebugOverlay.ColorizeText($"'{command}'", Colors.MediumVioletRed)}{DebugOverlay.ColorizeText(". Use it like this: ", Colors.Gray)}{parts[0]} {ArgTypesToString(cm.argTypes)}", true);
                return;
            }

            object[] typedArgs = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                switch (cm.argTypes[i])
                {
                    case ArgType.Int:
                        typedArgs[i] = int.Parse(args[i]);
                        break;

                    case ArgType.Float:
                        typedArgs[i] = float.Parse(args[i]);
                        break;

                    case ArgType.Bool:
                        typedArgs[i] = args[i] is "true" ? true : false;
                        break;

                    case ArgType.String:
                        typedArgs[i] = args[i];
                        break;
                }
            }

            cm.target.Callv(cm.method, new Godot.Collections.Array(typedArgs));

            bool ArgsValid()
            {
                if (args.Length != cm.argTypes.Length) return false;

                for (int i = 0; i < args.Length; i++)
                {
                    if (!ArgTypeMatch(args[i], cm.argTypes[i])) return false;

                    if (cm.argTypes[i] is ArgType.String) args[i] = args[i].Replace("%>space<%", " ").Replace("\"", "");
                }

                return true;
            }
        }

        private string ArgTypesToString(ArgType[] argTypes)
        {
            string result = "";

            for (int i = 0; i < argTypes.Length; i++)
            {
                if (i != 0 && i != argTypes.Length - 1)
                    result += " ";
                result += argTypes[i].ToString().ToLower();
            }

            return result;
        }

        private bool ArgTypeMatch(string toMatch, ArgType argType)
        {
            switch (argType)
            {
                case ArgType.Int: return int.TryParse(toMatch, out _);
                case ArgType.Float: return float.TryParse(toMatch, out _);
                case ArgType.Bool: return toMatch is "true" or "false";
                case ArgType.String: return toMatch.StartsWith("\"") && toMatch.EndsWith("\"");
                default: return false;
            }
        }

        #region Generic Commands

        private void PrintLine(string what)
        {
            DebugOverlay.AddOutputLine(what, true);
        }

        private void Help()
        {
            DebugOverlay.AddOutputLine("Available Commands:\n", true);

            foreach (var command in availableCommands)
            {
                if (command.Value.target == this && command.Value.method == nameof(Help)) continue;
                DebugOverlay.AddOutputLine($"- {command.Key} {ArgTypesToString(command.Value.argTypes)} {(command.Value.description is "" ? "" : DebugOverlay.ColorizeText($"<{command.Value.description}>", Colors.Gray))}", true);
            }
            DebugOverlay.AddOutputLine("", true);
        }

        private void ScreenshotDir()
        {
            DebugOverlay.AddOutputLine($"Your screenshot directory is here [url]{DebugOverlay.instance.Menu.screenshotDir}[/url]", true);
        }

        private async void SetScreenshotDir(string path)
        {
            Directory dir = new();

            if (!dir.DirExists(path))
            {
                string input = await Console.ReadLine($"{DebugOverlay.ColorizeText("directory {path} doesn't exists. Create it now?", Colors.Gray)}", "yes", "no");

                if (input is not "yes") return;

                Error error = dir.MakeDirRecursive(path);

                if (error is not Error.Ok)
                {
                    DebugOverlay.AddOutputLine($"Couldn't create the directory {DebugOverlay.instance.Menu.screenshotDir}, Eroor: {error}", true);
                    return;
                }

                DebugOverlay.instance.Menu.screenshotDir = path;
                DebugOverlay.AddOutputLine($"Succesfully created and set the screenshot directory to [url]{DebugOverlay.instance.Menu.screenshotDir}[/url]", true);
                return;
            }

            DebugOverlay.instance.Menu.screenshotDir = path;
            DebugOverlay.AddOutputLine($"Succesfully set the screenshot directory to [url]{DebugOverlay.instance.Menu.screenshotDir}[/url]", true);
        }

        private void DateInScreenshot(bool show)
        {
            DebugOverlay.instance.Menu.dateInScreenShot = show;
            DebugOverlay.AddOutputLine($"The date will now be {(show ? "shown" : "hidden")} in screenshots", true);
        }

        #endregion Generic commands
    }

    public struct Command
    {
        public Godot.Object target;
        public string method;
        public ArgType[] argTypes;
        public string description;

        public Command(Godot.Object target, string method, ArgType[] argTypes, string description = "")
        {
            this.target = target;
            this.method = method;
            this.argTypes = argTypes;
            this.description = description;
        }
    }

    public enum ArgType
    {
        Int,
        Float,
        Bool,
        String
    }
}
#endif