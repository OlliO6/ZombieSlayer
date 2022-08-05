#if DEBUG
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Additions.Debugging
{
    public class Commands : Node
    {
        public Dictionary<string, Command> availableCommands;
        public override void _Ready()
        {
            // Assembly assembly = typeof(Commands).Assembly;

            // foreach (TypeInfo type in assembly.DefinedTypes)
            // {
            //     type.GetCustomAttribute<>()
            // }

            availableCommands = new()
            {
                {
                    "print",
                    new Command(this, nameof(PrintLine), new ArgType[] { ArgType.String })
                },
                {
                    "log",
                    new Command(this, nameof(PrintLine), new ArgType[] { ArgType.String })
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
                GD.Print(command);
                command = command.Remove(0, 9).Remove(command.Length - 18);
                GD.Print(command);
            }

            command = command.StripEdges();

            string[] parts = command.Split(' ').Where((part) => part is not " " and not "").ToArray();

            if (parts.Length is 0) return;

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

            cm.target.Callv(cm.method, new Godot.Collections.Array(args));

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

        public void PrintLine(string what)
        {
            DebugOverlay.AddOutputLine(what, true);
        }
    }

    public struct Command
    {
        public Godot.Object target;
        public string method;
        public ArgType[] argTypes;

        public Command(Godot.Object target, string method, ArgType[] argTypes)
        {
            this.target = target;
            this.method = method;
            this.argTypes = argTypes;
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