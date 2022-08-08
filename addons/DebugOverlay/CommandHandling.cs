#if DEBUG
namespace Additions.Debugging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public class CommandHandling : Node
{
    public Dictionary<string, Command> availableCommands;
    private Console Console => DebugOverlay.instance.Console;

    public override void _EnterTree()
    {
        availableCommands = new()
            {
                {
                    "help",
                    new Command(this, nameof(Help), new ArgType[] { }, "#HIDEINHELP#")
                }
            };

        Assembly assembly = typeof(CommandHandling).Assembly;

        foreach (TypeInfo type in assembly.DefinedTypes)
        {
            bool correctType = type.BaseType == typeof(CommandCollection);

            if (!correctType) continue;

            CommandCollection commandClass = System.Activator.CreateInstance(type) as CommandCollection;

            commandClass.console = Console;
            commandClass.sceneTree = GetTree();

            // Found Command class
            DefaultColorAttribute color = type.GetCustomAttribute<DefaultColorAttribute>();
            availableCommands.Add(commandClass.CollectionName, Command.Header(commandClass.Description, color is null ? Colors.Wheat : color.unommonColor));

            foreach (MethodInfo method in type.DeclaredMethods)
            {
                CommandAttribute command = method.GetCustomAttribute<CommandAttribute>();
                if (command is null) continue;

                ParameterInfo[] parameters = method.GetParameters();
                ArgType[] args = new ArgType[parameters.Length];
                bool paramValid = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(int))
                    {
                        args[i] = ArgType.Int;
                        continue;
                    }
                    if (parameters[i].ParameterType == typeof(float))
                    {
                        args[i] = ArgType.Float;
                        continue;
                    }
                    if (parameters[i].ParameterType == typeof(bool))
                    {
                        args[i] = ArgType.Bool;
                        continue;
                    }
                    if (parameters[i].ParameterType == typeof(string))
                    {
                        args[i] = ArgType.String;
                        continue;
                    }

                    GD.PrintErr($"Cant construct command because of invalid parameter of type {parameters[i].ParameterType.ToString()}. Valid types are 'int', 'float', 'bool' and 'string'");
                    paramValid = false;
                    break;
                }
                if (!paramValid) continue;

                availableCommands.Add(
                    command.commandName.ToLower(),
                    new Command(commandClass, method.Name, args, command.description, color is null ? Colors.White : color.commonColor)
                );
            }

        }
    }

    private void Help()
    {
        foreach (var command in availableCommands)
        {
            if (command.Value.description is "#HIDEINHELP#") continue;

            if (command.Value.method is "#HEADER#")
            {
                DebugOverlay.AddOutputLine($"{DebugOverlay.ColorizeText(command.Key, command.Value.helpTint)}: {(command.Value.description is "" ? "" : DebugOverlay.ColorizeText($"{command.Value.description}", Colors.Gray))}", true);

                continue;
            }
            DebugOverlay.AddOutputLine($"- {DebugOverlay.ColorizeText(command.Key, command.Value.helpTint)} {ArgTypesToString(command.Value.argTypes)} {(command.Value.description is "" ? "" : DebugOverlay.ColorizeText($"{command.Value.description}", Colors.DarkGray))}", true);
        }
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

        if (!availableCommands.Any((KeyValuePair<string, Command> command) => (command.Key.ToLower() == parts[0])) || availableCommands[parts[0]].method is "#HEADER#")
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
}

public struct Command
{
    public Godot.Object target;
    public string method;
    public ArgType[] argTypes;
    public string description;
    public Color helpTint;

    public Command(Godot.Object target, string method, ArgType[] argTypes, string description = "")
    {
        this.target = target;
        this.method = method;
        this.argTypes = argTypes;
        this.description = description;
        this.helpTint = Colors.White;
    }

    public Command(Object target, string method, ArgType[] argTypes, string description, Color helpTint) : this(target, method, argTypes, description)
    {
        this.helpTint = helpTint;
    }

    public static Command Header(string description, Color tint) => new Command() { method = "#HEADER#", description = description, helpTint = tint };
}

public enum ArgType
{
    Int,
    Float,
    Bool,
    String
}
#endif