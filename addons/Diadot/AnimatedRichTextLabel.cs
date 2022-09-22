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
    public event Func<string, Task> NotHandeledExpression;

    public float delay;

    private CancellationTokenSource cancellation;

    public async void Play(string codeText)
    {
        cancellation?.Cancel();
        cancellation = new CancellationTokenSource();
        CancellationToken token = cancellation.Token;

        BbcodeText = codeText;
        FilterExpressions(out Dictionary<int, List<string>> expressions);

        delay = ProjectSettingsControl.DefaultDelay;
        VisibleCharacters = 0;

        for (int i = 0; i < Text.Length; i++)
        {
            char c = Text[i];

            if (c is '\n') continue;

            if (expressions.ContainsKey(i))
            {
                foreach (string expression in expressions[i])
                {
                    await ProcessExpression(expression);
                    if (token.IsCancellationRequested) return;
                }
            }

            VisibleCharacters++;

            if (c is not ' ') EmitSignal(nameof(NonWhiteSpaceAdvanced));
            EmitSignal(nameof(Advanced));

            await new TimeAwaiter(this, delay * (Input.IsActionPressed(ProjectSettingsControl.SkipInput) ? ProjectSettingsControl.DelayFactorWhenSkipPressed : 1));
            if (token.IsCancellationRequested) return;
        }
        if (token.IsCancellationRequested) return;

        EmitSignal(nameof(Finished));


        void FilterExpressions(out Dictionary<int, List<string>> expressions)
        {
            RemoveFromText(out expressions);
            RemoveFromBbText();

            void RemoveFromText(out Dictionary<int, List<string>> expressions)
            {
                expressions = new();

                string newText = Text;
                int skipped = 0;

                for (int i = 0; i < Text.Length; i++)
                {
                    char c = Text[i];

                    if (c is '\\')
                    {
                        string expression = string.Empty;
                        int startIndex = i;

                        while (true)
                        {
                            i++;
                            if (i >= Text.Length) return;

                            c = Text[i];
                            if (c is '/')
                                break;
                            expression += c;
                        }
                        int expressionIndex = startIndex - skipped;
                        newText = newText.Remove(expressionIndex, i - startIndex);
                        AddExpression(ref expressions, expressionIndex, expression);

                        skipped += i + 1 - startIndex;
                    }
                }
                Text = newText;

                void AddExpression(ref Dictionary<int, List<string>> expressions, int index, string expression)
                {
                    if (expressions.ContainsKey(index))
                    {
                        expressions[index].Add(expression);
                        return;
                    }
                    expressions.Add(index, new() { expression });
                }
            }

            void RemoveFromBbText()
            {
                string newBb = BbcodeText;
                int skipped = 0;

                for (int i = 0; i < BbcodeText.Length; i++)
                {
                    char c = BbcodeText[i];

                    if (c is '\\')
                    {
                        int startIndex = i;
                        string expression = string.Empty;

                        while (true)
                        {
                            i++;
                            if (i >= BbcodeText.Length) return;
                            expression += BbcodeText[i];
                            if (BbcodeText[i] is '/')
                                break;
                        }
                        newBb = newBb.Remove(startIndex - skipped, i + 1 - startIndex);
                        skipped += i + 1 - startIndex;
                    }
                }
                BbcodeText = newBb;
            }
        }
    }

    private async Task ProcessExpression(string expression)
    {
        bool v = await TryToHandleExpression();

        if (v) return;

        await NotHandeledExpression?.Invoke(expression);

        async Task<bool> TryToHandleExpression()
        {
            if (expression.Contains("="))
            {
                string[] equalSeperated = expression.Split('=');

                if (equalSeperated.Length < 2) return false;

                switch (equalSeperated[0])
                {
                    case "d":
                        delay = ParseFloat(equalSeperated[1]);
                        return true;

                    // Project specific
                    case "input":
                        InputManager.ProcessInput = bool.Parse(equalSeperated[1]);
                        return true;
                }
                return false;
            }

            string[] spaceSeperated = expression.Split(' ');
            if (spaceSeperated.Length < 1) return false;

            switch (spaceSeperated[0])
            {
                case "reset":
                    if (spaceSeperated.Length < 2) return false;
                    switch (spaceSeperated[1])
                    {
                        case "d":
                            delay = ProjectSettingsControl.DefaultDelay;
                            return true;
                    }
                    break;

                case "wait":
                    if (spaceSeperated.Length < 2) return false;
                    await new TimeAwaiter(this, ParseFloat(spaceSeperated[1]) * (Input.IsActionPressed(ProjectSettingsControl.SkipInput) ? ProjectSettingsControl.DelayFactorWhenSkipPressed : 1));
                    return true;
            }
            return false;
        }
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

    public void CancelPlay() => cancellation?.Cancel();
}
