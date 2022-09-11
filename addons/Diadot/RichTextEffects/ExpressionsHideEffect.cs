namespace Diadot;
using System.Collections.Generic;
using Additions;
using Godot;

[Tool]
public class ExpressionsHideEffect : RichTextEffect
{
    public string bbcode = "expressions";

    private bool isInBackslashes;

    public override bool _ProcessCustomFx(CharFXTransform charFx)
    {
        if (charFx.RelativeIndex is 0) isInBackslashes = false;

        if (isInBackslashes)
        {
            charFx.Visible = false;

            if (char.ConvertFromUtf32(charFx.Character) is "/")
            {
                isInBackslashes = false;
            }
            return true;
        }

        if (char.ConvertFromUtf32(charFx.Character) is "\\")
        {
            isInBackslashes = true;
            charFx.Visible = false;
        }
        return true;
    }
}
