using System.Collections.Generic;
using Additions;
using Godot;

public class OptionSet
{
    public OptionSet() { }

    public bool fullscreen = true;
    public float sfxVolume = 0.6f, musicVolume = 0.6f;
    public bool useUpscaling = false;
    public string language = "en";
}
