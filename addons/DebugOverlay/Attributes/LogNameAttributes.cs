namespace Additions.Debugging;
using System;
using Godot;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class LogNameAttribute : Attribute
{
    public string name;

    public LogNameAttribute(string name)
    {
        this.name = name;
    }
}