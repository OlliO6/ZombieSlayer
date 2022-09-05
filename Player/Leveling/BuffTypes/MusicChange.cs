namespace Leveling.Buffs;
using System.Collections.Generic;
using Additions;
using Godot;

public class MusicChange : LevelBuff
{
    [Export] public AudioStream song;
    [Export] public float fadeOutTime = 0.5f, delay = 0, fadeInTime = 0;

    public override void Apply()
    {
        AudioStreamPlayer musicPlayer = GetTree().CurrentScene.GetNode<AudioStreamPlayer>("Music");

        SceneTreeTween tween = CreateTween();
        tween.TweenMethod(this, nameof(SetMusicVolume), 1f, 0f, fadeOutTime, new(musicPlayer));
        tween.TweenInterval(delay);
        tween.SetParallel(false);

        ToSignal(tween, "finished").OnCompleted(() =>
        {
            musicPlayer.Stream = song;
            musicPlayer.Play();

            if (fadeInTime is 0)
            {
                musicPlayer.VolumeDb = GD.Linear2Db(1);
                return;
            }

            tween = CreateTween();
            tween.TweenMethod(this, nameof(SetMusicVolume), 0f, 1f, fadeInTime, new(musicPlayer));
        });
    }

    public override string GetBuffText() => "";

    private void SetMusicVolume(float linearVolume, AudioStreamPlayer player) => player.VolumeDb = GD.Linear2Db(linearVolume);
}
