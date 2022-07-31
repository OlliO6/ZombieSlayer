using System;
using Godot;

public class OptionsManager : Node
{
    public static OptionsManager instance;

    [Signal] public delegate void OptionsChanged();

    public static Shader[] spriteShaders = new Shader[]
    {
        GD.Load<Shader>("res://Shaders/Standart.shader"),
        GD.Load<Shader>("res://Shaders/Flash.shader"),
        GD.Load<Shader>("res://Shaders/Player.shader"),
    };
    private static OptionSet currentOptions;

    const string DefaultOptionsPath = "res://Singletons/Options/DefaultOptions.tres";
    const string UserOptionsPath = "user://Options.tres";

    #region Current Options Shortcuts 

    public static OptionSet CurrentOptions
    {
        get => currentOptions;
        set
        {
            currentOptions = value;
            UpdateOptions();
        }
    }

    public static bool IsFullscreen
    {
        get => CurrentOptions.fullscreen;
        set
        {
            CurrentOptions.fullscreen = value;
            OS.WindowFullscreen = value;
            instance.EmitSignal(nameof(OptionsChanged));
        }
    }
    public static float SfxVolume
    {
        get => CurrentOptions.sfxVolume;
        set
        {
            CurrentOptions.sfxVolume = value;
            AudioServer.SetBusVolumeDb(1, GD.Linear2Db(SfxVolume));
            instance.EmitSignal(nameof(OptionsChanged));
        }
    }
    public static float MusicVolune
    {
        get => CurrentOptions.musicVolume;
        set
        {
            CurrentOptions.musicVolume = value;
            AudioServer.SetBusVolumeDb(2, GD.Linear2Db(MusicVolune));
            instance.EmitSignal(nameof(OptionsChanged));
        }
    }
    public static bool IsUpscaling
    {
        get => CurrentOptions.useUpscaling;
        set
        {
            if (value == IsUpscaling) return;
            CurrentOptions.useUpscaling = value;
            instance.UpdateShaders();
            instance.EmitSignal(nameof(OptionsChanged));
        }
    }

    #endregion Current Options Shortcuts

    public override void _EnterTree()
    {
        instance = this;

        SetToUserFile();
        UpdateOptions();
        UpdateShaders();
    }
    public override void _ExitTree()
    {
        SaveOptions();
    }

    public static void SetToUserFile()
    {
        if (!ResourceLoader.Exists(UserOptionsPath))
        {
            CurrentOptions = GD.Load<OptionSet>(DefaultOptionsPath).Duplicate() as OptionSet;
            ResourceSaver.Save(UserOptionsPath, CurrentOptions);
            return;
        }

        CurrentOptions = ResourceLoader.Load<OptionSet>(UserOptionsPath, noCache: true);
    }

    public static void ResetOptions() => CurrentOptions = GD.Load<OptionSet>(DefaultOptionsPath).Duplicate() as OptionSet;
    public static void SaveOptions() => ResourceSaver.Save(UserOptionsPath, CurrentOptions);

    public static void UpdateOptions()
    {
        instance.SetBlockSignals(true);

        // Call Setters
        IsFullscreen = IsFullscreen;
        SfxVolume = SfxVolume;
        MusicVolune = MusicVolune;
        IsUpscaling = IsUpscaling;

        instance.SetBlockSignals(false);
        instance.EmitSignal(nameof(OptionsChanged));
    }

    private void UpdateShaders()
    {
        #region Shader code 

        const string StartOptions = "// Start Option Methods";
        const string EndOptions = "// End Option Methods";

        const string UpscaleCode = @"const int ML = 0;
const float THRESHOLD = 0.05;
const float AA_SCALE = 10.0;

vec4 texelGet ( sampler2D tg_tex, ivec2 tg_coord, int tg_lod ) {
	vec2 tg_texel = 1.0 / vec2(textureSize(tg_tex, 0));
	vec2 tg_getpos = (vec2(tg_coord) * tg_texel) + (tg_texel * 0.5);
	return texture(tg_tex, tg_getpos, float(tg_lod));
}

vec4 diag(vec4 sum, vec2 uv, vec2 p1, vec2 p2, sampler2D iChannel0, float LINE_THICKNESS) {
	vec4 v1 = texelGet(iChannel0,ivec2(uv+vec2(p1.x,p1.y)),ML),
		v2 = texelGet(iChannel0,ivec2(uv+vec2(p2.x,p2.y)),ML);
	if (length(v1-v2) < THRESHOLD) {
		vec2 dir = p2-p1,
			lp = uv-(floor(uv+p1)+.5);
		dir = normalize(vec2(dir.y,-dir.x));
		float l = clamp((LINE_THICKNESS-dot(lp,dir))*AA_SCALE,0.,1.);
		sum = mix(sum,v1,l);
	}
	return sum;
}
        
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
	//line thickness
	float LINE_THICKNESS = 0.4;
	vec2 ip = uv;
	ip = uv * (1.0 / pixel_size);

	//start with nearest pixel as 'background'
	vec4 s = texelGet(text, ivec2(ip), ML);

	//draw anti aliased diagonal lines of surrounding pixels as 'foreground'
	s = diag(s,ip,vec2(-1,0),vec2(0,1), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(0,1),vec2(1,0), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(1,0),vec2(0,-1), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(0,-1),vec2(-1,0), text, LINE_THICKNESS);
	
	return s;
}";
        const string NormalCode = @"vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}";

        #endregion Shader code

        string optionsCode = IsUpscaling ? UpscaleCode : NormalCode;

        foreach (Shader shader in spriteShaders)
        {
            string code = shader.Code;

            int startPos = code.IndexOf(StartOptions) + StartOptions.Length;
            int endPos = code.IndexOf(EndOptions);

            code = $"{code.Remove(startPos)}\n{optionsCode}\n{code.Remove(0, endPos)}";

            shader.Code = code;
        }
    }
}
