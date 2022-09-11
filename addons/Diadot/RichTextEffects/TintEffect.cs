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
        float r = GetOrDefault(charFx.Env, "r", 1f);
        float g = GetOrDefault(charFx.Env, "g", 1f);
        float b = GetOrDefault(charFx.Env, "b", 1f);
        float a = GetOrDefault(charFx.Env, "a", 1f);

        charFx.Color = new Color(r, g, b, a);
        return true;
    }

    public T GetOrDefault<T>(Godot.Collections.Dictionary dict, object key, T defaultValue, bool throwOnInvalidCast = true)
    {
        if (dict is null) return defaultValue;
        if (throwOnInvalidCast)
            return dict.Contains(key) ? (T)(object)dict[key] : defaultValue;
        return dict.Contains(key) ? (dict[key] is T value ? value : defaultValue) : defaultValue;
    }
}
