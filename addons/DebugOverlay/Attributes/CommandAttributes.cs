namespace Additions.Debugging;

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class CommandAttribute : System.Attribute
{
    public string commandName, description = "";
    public CommandAttribute(string commandName) => this.commandName = commandName;
    public CommandAttribute(string commandName, string description) : this(commandName) => this.description = description;
}