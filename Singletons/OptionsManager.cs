using Godot;
using System;

public class OptionsManager : Node
{
    public static OptionsManager instance;

    public override void _Ready()
    {
        instance = this;
        UpdateOptions();
    }

    public static int sfxVolume = 7;
    public static int musicVolume = 7;
    public static bool fullscreen = false;

    public void UpdateOptions()
    {
        OS.WindowFullscreen = fullscreen;

        GD.Print(AudioServer.BusCount);


        if (sfxVolume is 0)
        {
            AudioServer.SetBusMute(1, true);
        }
        else
        {
            AudioServer.SetBusMute(1, false);
            AudioServer.SetBusVolumeDb(1, (Mathf.Pow(sfxVolume, 0.2f) - Mathf.Pow(10, 0.2f)) * 130f + 6); // Weird calculation because decibles are weird
        }

        if (musicVolume is 0)
        {
            AudioServer.SetBusMute(2, true);
        }
        else
        {
            AudioServer.SetBusMute(2, false);
            AudioServer.SetBusVolumeDb(2, (Mathf.Pow(musicVolume, 0.2f) - Mathf.Pow(10, 0.2f)) * 130f + 6);
        }
    }
}
