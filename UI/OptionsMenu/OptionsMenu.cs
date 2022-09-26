using System;
using Godot;

public class OptionsMenu : Control
{
    [Export] private NodePath _fullscreenToggle, _sfxVolumeSlider, _musicVolumeSlider, _upscaleToggle;

    private bool open;
    private CheckButton fullscreenToggle;
    private Slider sfxSlider, musicSlider;
    private CheckButton upscaleToggle;
    public override void _Ready()
    {
        OptionsManager.instance.Connect(nameof(OptionsManager.OptionsChanged), this, nameof(ReloadOptions));

        fullscreenToggle = GetNode<CheckButton>(_fullscreenToggle);
        sfxSlider = GetNode<Slider>(_sfxVolumeSlider);
        musicSlider = GetNode<Slider>(_musicVolumeSlider);
        upscaleToggle = GetNode<CheckButton>(_upscaleToggle);

        fullscreenToggle.Connect("toggled", this, nameof(SetFullscreen));
        sfxSlider.Connect("value_changed", this, nameof(SetSfxVolume));
        musicSlider.Connect("value_changed", this, nameof(SetMusicVolume));
        upscaleToggle.Connect("toggled", this, nameof(SetUseUpscaling));
    }

    public override void _EnterTree()
    {
        InputManager.UICancelPressed += OnUICancelPressed;
    }
    public override void _ExitTree()
    {
        InputManager.UICancelPressed -= OnUICancelPressed;
    }

    private void OnUICancelPressed()
    {
        if (open)
            OnBackPressed();
    }

    private void ReloadOptions()
    {
        if (!Visible) return;

        fullscreenToggle.SetBlockSignals(true);
        sfxSlider.SetBlockSignals(true);
        musicSlider.SetBlockSignals(true);
        upscaleToggle.SetBlockSignals(true);

        fullscreenToggle.Pressed = OptionsManager.IsFullscreen;
        sfxSlider.Value = OptionsManager.SfxVolume;
        musicSlider.Value = OptionsManager.MusicVolune;
        upscaleToggle.Pressed = OptionsManager.IsUpscaling;

        fullscreenToggle.SetBlockSignals(false);
        sfxSlider.SetBlockSignals(false);
        musicSlider.SetBlockSignals(false);
        upscaleToggle.SetBlockSignals(false);
    }

    [TroughtEditor]
    private void OnOptionsPressed()
    {
        Visible = true;
        open = true;

        ReloadOptions();
    }

    [TroughtEditor]
    private void OnBackPressed()
    {
        Visible = false;
        open = false;

        OptionsManager.SaveOptions();
    }

    [TroughtEditor]
    private void OnRevertPressed() => OptionsManager.SetToUserFile();

    [TroughtEditor]
    private void OnResetPressed() => OptionsManager.ResetOptions();

    private void SetFullscreen(bool value) => OptionsManager.IsFullscreen = value;
    private void SetMusicVolume(float value) => OptionsManager.MusicVolune = value;
    private void SetUseUpscaling(bool value) => OptionsManager.IsUpscaling = value;
    private void SetSfxVolume(float value)
    {
        OptionsManager.SfxVolume = value;
        GetNode<AudioStreamPlayer>("SfxSoundPlayer").Play();
    }
}
