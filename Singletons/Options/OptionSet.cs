using System.Collections.Generic;
using Additions;
using Godot;

public class OptionSet : Resource
{
    [Export] public bool fullscreen;
    [Export] public float sfxVolume, musicVolume;
    [Export] public bool useUpscaling;
    [Export] public string language = "en";
}
