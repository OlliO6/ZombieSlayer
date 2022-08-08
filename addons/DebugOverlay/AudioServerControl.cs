#if DEBUG
namespace Additions.Debugging;
using Godot;

public class AudioServerControl : VBoxContainer
{
    private PackedScene audioBusCtrlScene = GD.Load<PackedScene>("res://addons/DebugOverlay/AudioBusControl.tscn");

    [TroughtSignal]
    public void UpdateLayout()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
        for (int i = 0; i < AudioServer.BusCount; i++)
        {
            Node audioBusControl = audioBusCtrlScene.Instance();
            AddChild(audioBusControl);

            audioBusControl.GetNode<Label>("Label").Text = AudioServer.GetBusName(i);
            Slider slider = audioBusControl.GetNode<Slider>("Slider");
            slider.Value = GD.Db2Linear(AudioServer.GetBusVolumeDb(i));
            slider.Connect("value_changed", this, nameof(VolumeSliderChanged), new(i));
        }
    }

    private void VolumeSliderChanged(float value, int bus)
    {
        AudioServer.SetBusVolumeDb(bus, GD.Linear2Db(value));
    }
}

#endif