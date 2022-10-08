namespace Diadot;
using System.Collections.Generic;
using Additions;
using Godot;

// [tint r=1 g=1 b=1 a=1][/tint]
[Tool]
public class TintEffect : RichTextEffect
{
    public string bbcode = "tint";

    public override bool _ProcessCustomFx(CharFXTransform charFx)
    {
        float r = charFx.Env.GetOrDefault("r", 1f);
        float g = charFx.Env.GetOrDefault("g", 1f);
        float b = charFx.Env.GetOrDefault("b", 1f);
        float a = charFx.Env.GetOrDefault("a", 1f);

        charFx.Color = new Color(r, g, b, a);
        return true;
    }
}
