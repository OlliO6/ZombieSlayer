namespace Diadot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Additions;
using Godot;

public class AnimatedRichTextLabel : RichTextLabel
{
    [Signal] public delegate void Advanced();
    [Signal] public delegate void NonWhiteSpaceAdvanced();
    [Signal] public delegate void Finished();

    public float delay;

    private CancellationTokenSource cancellation;

    public async void Play(string bbText = null)
    {
        cancellation?.Cancel();
        cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;

        if (bbText is not null) BbcodeText = bbText;

        delay = ProjectSettingsControl.DefaultDelay;
        VisibleCharacters = 0;
        string textString = Text.Replace("\n", "");

        while (VisibleCharacters < textString.Length)
        {
            VisibleCharacters++;
            char lastChar = textString[VisibleCharacters - 1];

            if (lastChar is '\\')
            {
                string expression = "";

                while (true)
                {
                    VisibleCharacters++;
                    char c = textString[VisibleCharacters - 1];
                    if (c is '/')
                        break;
                    expression += c;
                }

                await ProcessExpression(expression);
                if (token.IsCancellationRequested) return;
                continue;
            }

            if (!(lastChar is ' ')) EmitSignal(nameof(NonWhiteSpaceAdvanced));
            EmitSignal(nameof(Advanced));
            await new TimeAwaiter(this, delay * (Input.IsActionPressed(ProjectSettingsControl.SkipInput) ? ProjectSettingsControl.DelayFactorWhenSkipPressed : 1));
            if (token.IsCancellationRequested) return;
        }
        if (token.IsCancellationRequested) return;

        EmitSignal(nameof(Finished));
    }

    private async Task ProcessExpression(string expression)
    {
        if (expression.Contains("="))
        {
            string[] equalSeperated = expression.Split('=');

            if (equalSeperated.Length < 2) return;

            switch (equalSeperated[0])
            {
                case "d":
                    delay = ParseFloat(equalSeperated[1]);
                    break;
            }
            return;
        }

        string[] spaceSeperated = expression.Split(' ');
        if (spaceSeperated.Length < 1) return;

        switch (spaceSeperated[0])
        {
            case "reset":
                if (spaceSeperated.Length < 2) return;
                switch (spaceSeperated[1])
                {
                    case "d":
                        delay = ProjectSettingsControl.DefaultDelay;
                        break;
                }
                break;

            case "wait":
                if (spaceSeperated.Length < 2) return;
                await new TimeAwaiter(this, ParseFloat(spaceSeperated[1]) * (Input.IsActionPressed(ProjectSettingsControl.SkipInput) ? ProjectSettingsControl.DelayFactorWhenSkipPressed : 1));
                break;
        }
        return;
    }

    private static float ParseFloat(string number) => float.Parse(number, System.Globalization.CultureInfo.InvariantCulture);

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseInput)
        {
            if (mouseInput.Pressed)
            {
                if (GetGlobalRect().HasPoint(mouseInput.Position))
                {
                    Input.ParseInputEvent(new InputEventAction()
                    {
                        Action = ProjectSettingsControl.SkipInput,
                        Pressed = true
                    });
                }
                return;
            }
            Input.ParseInputEvent(new InputEventAction()
            {
                Action = ProjectSettingsControl.SkipInput,
                Pressed = false
            });
        }
    }
}
