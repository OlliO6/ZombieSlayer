using Godot;
using System;

[Tool]
public class RngAudioPlayer : AudioStreamPlayer
{
    [Export] public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [Export] public Vector2 volumeRange = new Vector2(-3, 1);
    [Export] public Godot.Collections.Array<AudioStream> streams;
    [Export] public bool mute;

#if TOOLS
    [Export]
    public bool Test
    {
        get => false;
        set => PlayRandom();
    }
#endif

    public void PlayRandom()
    {
        if (mute)
            return;

        Stream = streams[(int)(GD.Randi() % streams.Count + 0)];
        PitchScale = (float)GD.RandRange((float)pitchRange.x, (float)pitchRange.y);
        VolumeDb = (float)GD.RandRange((float)volumeRange.x, (float)volumeRange.y);
        Play();
    }
}
