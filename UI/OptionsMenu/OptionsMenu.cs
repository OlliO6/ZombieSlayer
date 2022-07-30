using Godot;
using System;

public class OptionsMenu : Control
{
    [Export] private NodePath _fullscreenToggle, _sfxVolumeSlider, _musicVolumeSlider, _upscaleToggle;

    private CheckButton fullscreenToggle;
    private Slider sfxSlider, musicSlider;
    private CheckButton upscaleToggle;
    public override void _Ready()
    {
        fullscreenToggle = GetNode<CheckButton>(_fullscreenToggle);
        sfxSlider = GetNode<Slider>(_sfxVolumeSlider);
        musicSlider = GetNode<Slider>(_musicVolumeSlider);
        upscaleToggle = GetNode<CheckButton>(_upscaleToggle);

        fullscreenToggle.Connect("toggled", this, nameof(SetFullscreen));
        sfxSlider.Connect("value_changed", this, nameof(SetSfxVolume));
        musicSlider.Connect("value_changed", this, nameof(SetMusicVolume));
        upscaleToggle.Connect("toggled", this, nameof(SetUseUpscaling));
    }

    [TroughtSignal]
    private void OnOptionsPressed()
    {
        Visible = true;

        fullscreenToggle.SetBlockSignals(true);
        sfxSlider.SetBlockSignals(true);
        musicSlider.SetBlockSignals(true); ;
        upscaleToggle.SetBlockSignals(true);

        fullscreenToggle.Pressed = OptionsManager.fullscreen;
        sfxSlider.Value = OptionsManager.sfxVolume;
        musicSlider.Value = OptionsManager.musicVolume;
        upscaleToggle.Pressed = OptionsManager.useUpscaling;

        fullscreenToggle.SetBlockSignals(false);
        sfxSlider.SetBlockSignals(false);
        musicSlider.SetBlockSignals(false); ;
        upscaleToggle.SetBlockSignals(false);
    }

    [TroughtSignal]
    private void OnBackPressed()
    {
        Visible = false;
    }

    private void SetFullscreen(bool value)
    {
        OptionsManager.fullscreen = value;
        OptionsManager.UpdateOptions();
    }

    private void SetSfxVolume(float value)
    {
        OptionsManager.sfxVolume = value;
        OptionsManager.UpdateOptions();

        GetNode<AudioStreamPlayer>("SfxSoundPlayer").Play();
    }

    private void SetMusicVolume(float value)
    {
        OptionsManager.musicVolume = value;
        OptionsManager.UpdateOptions();
    }

    private void SetUseUpscaling(bool value)
    {
        OptionsManager.useUpscaling = value;

        OptionsManager.UpdateShaders();
    }
}
