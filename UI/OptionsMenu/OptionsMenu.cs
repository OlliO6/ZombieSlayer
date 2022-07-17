using Godot;
using System;

public class OptionsMenu : Control
{
    [Export] private NodePath fullscreenToggle, sfxVolumeSlider, musicVolumeSlider;

    [TroughtSignal]
    private void OnOptionsPressed()
    {
        Visible = true;

        GetNode<CheckButton>(fullscreenToggle).Pressed = OptionsManager.fullscreen;
        GetNode<Slider>(sfxVolumeSlider).Value = OptionsManager.sfxVolume;
        GetNode<Slider>(musicVolumeSlider).Value = OptionsManager.musicVolume;
    }

    [TroughtSignal]
    private void OnBackPressed()
    {
        Visible = false;
    }

    [TroughtSignal]
    private void OnFullscreenToggled(bool toggled)
    {
        OptionsManager.fullscreen = toggled;

        OptionsManager.instance.UpdateOptions();
    }

    [TroughtSignal]
    private void OnSfxVolumeChanged(float value)
    {
        OptionsManager.sfxVolume = (int)value;

        OptionsManager.instance.UpdateOptions();
        GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
    }

    [TroughtSignal]
    private void OnMusicVolumeChanged(float value)
    {
        OptionsManager.musicVolume = (int)value;

        OptionsManager.instance.UpdateOptions();
    }
}
