using System.Collections.Generic;
using System.Threading;
using Additions;
using Godot;

[Tool]
public class AnimatedLabel : Label
{
    [Export] public AudioStream sound;

    #region AudioPlayer Reference

    private AudioStreamPlayer storerForAudioPlayer;
    public AudioStreamPlayer AudioPlayer => this.LazyGetNode(ref storerForAudioPlayer, "AudioPlayer", () => AudioPlayer.Stream = sound);

    #endregion

    private CancellationTokenSource cancellation;

    public void NewText(string text, float delayPerStep, TextAnimationMode mode = TextAnimationMode.Characters)
    {
        Text = text;

        AnimateText(delayPerStep, mode);
    }
    public void AnimateText(float delayPerStep, TextAnimationMode mode = TextAnimationMode.Characters)
    {
        switch (mode)
        {
            case TextAnimationMode.Characters:
                AnimateCharacters(delayPerStep);
                break;

            case TextAnimationMode.Words:
                AnimateWords(delayPerStep);
                break;
        }
    }

    private async void AnimateCharacters(float delayPerChar)
    {
        cancellation?.Cancel();
        cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;

        VisibleCharacters = 0;

        while (PercentVisible < 1)
        {
            if (token.IsCancellationRequested) return;

            VisibleCharacters++;
            AudioPlayer.Play();
            await new TimeAwaiter(this, delayPerChar);
        }
    }

    private async void AnimateWords(float delayPerWord)
    {
        cancellation?.Cancel();
        cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;

        string[] words = Text.Split(' ');
        Text = "";

        PercentVisible = 1;

        for (int i = 0; i < words.Length; i++)
        {
            if (token.IsCancellationRequested) return;

            Text += $"{words[i]} ";
            AudioPlayer.Play();

            await new TimeAwaiter(this, delayPerWord);
        }
    }

    public enum TextAnimationMode
    {
        Characters,
        Words
    }
}
